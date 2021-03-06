﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using DougKlassen.Revit.Automation;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using DougKlassen.Revit.Cron.Rotogravure.Interface;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	/// <summary>
	/// The handler to process batched tasks, with a callback that is invoked when Revit starts up
	/// </summary>
	public static class TaskProcessingLogic
	{
		static RevitHandler revitHandler = RevitHandler.Instance;
		static RCronLog log = RCronLog.Instance;
		static RCronOptions options;

		/// <summary>
		/// When Revit is started, check for a batch repo, load the batch, and run it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
		{
			try
			{
				options = RCronOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
			}
			catch (Exception exc)
			{
				options = new RCronOptions();
				log.LogException(exc);
			}

			var app = (Application)sender;
			AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
			log.AppendLine("**{0} Rotogravure initialized", DateTime.Now);
			log.AppendLine("  -- assembly: {0}", asm.Name);
			log.AppendLine("  -- version: {0}", asm.Version.ToString());

			RCronBatch batch;
			RCronBatchJsonRepo batchRepo;
			try
			{
				if (!File.Exists(options.BatchFileUri.LocalPath))
				{
					log.AppendLine("** no batch file found, processing complete");
					return;
				}

				batchRepo = new RCronBatchJsonRepo(options.BatchFileUri);	//todo: account for multiple batch files
				batch = batchRepo.GetRCronBatch();
			}
			catch (Exception exception)
			{
				log.AppendLine("!! Couldn't load batch from {0}", options.BatchFileUri.LocalPath);
				log.LogException(exception);
				LogWindow.Show(log);
				throw exception; //can't continue if the TasksRepo can't be loaded
			}

			log.AppendLine("  -- number of tasks found: {0}", batch.TaskSpecs.Count());
			revitHandler.AddDialogOverride(RevitDialog.DefaultFamilyTemplateInvalid, 8); //respond with "Close": IDCLOSE = 8 TODO: currently not working
			revitHandler.AddDialogOverride(RevitDialog.LostOnImport, 2); //respond with Ok: IDOK = 1
			revitHandler.AddDialogOverride(RevitDialog.UnresolvedReferences, 1002); //respond with "Ignore and continue opening the project"
			var firstTask = batch.TaskSpecs.Values.First();
			if (firstTask is RCronAuditCompactTaskSpec) //audit and compact tasks are batched separately and will open their own doc
			{
				log.AppendLine("\n** running audit and compact task");
				RunAuditCompactTask(firstTask as RCronAuditCompactTaskSpec, app);
			}
			else if (firstTask is RCronTestTaskSpec)
			{
				log.AppendLine("\n** running test task");
			}
			else //other commands will use an already open doc
			{
				var docPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(firstTask.ProjectFile);
				var openOpts = new OpenOptions();
				if (docPath.ServerPath)
				{
					openOpts.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
				}
				var uiApp = new UIApplication(app);
				Document dbDoc;

				if (batch.TaskSpecs.Values.Where(t => t is RCronPrintTaskSpec).Count() > 0) //if the batch contains a PrintTask
				{
					try
					{
						var uiDoc = uiApp.OpenAndActivateDocument(docPath, openOpts, false); //We need to open and activate the doc at the ui level or the Bluebeam print driver will crash when run on VDI. Can't use dbDoc = app.OpenDocumentFile(docPath, openOpts);
						dbDoc = uiDoc.Document;
					}
					catch(Exception exc)
					{
						log.LogException(exc);
						return; //todo: improve behavior when file isn't found
					}
				}
				else //otherwise open normally
				{
					dbDoc = app.OpenDocumentFile(docPath, openOpts);
				}

				foreach (RCronTaskSpec taskSpec in batch.TaskSpecs.Values)
				{
					try
					{
						switch (taskSpec.TaskType)
						{
							case RCronTaskType.Print:
								log.AppendLine("\n** running print task");
								RunPrintTask(taskSpec as RCronPrintTaskSpec, dbDoc);
								break;
							case RCronTaskType.Export:
								log.AppendLine("\n** running export task");
								RunExportTask(taskSpec as RCronExportTaskSpec, dbDoc);
								break;
							case RCronTaskType.ETransmit:
								log.AppendLine("\n** running eTransmit task");
								RunETransmitTask(taskSpec as RCronETransmitTaskSpec, dbDoc);
								break;
							case RCronTaskType.Command:
								log.AppendLine("\n** running command task");
								RunCommandTask(taskSpec as RCronCommandTaskSpec, dbDoc);
								break;
							default:
								break;
						}
					}
					catch (Exception exception)
					{
						log.LogException(exception);
					}
				}
			}

			revitHandler.Exit(synchronize: false, save: false); //close Revit without saving or synchronizing any open documents
			log.AppendLine("** all tasks completed");
		}

		private static void RunAuditCompactTask(RCronAuditCompactTaskSpec auditTask, Application app)
		{
			Boolean isCentralFile;

			log.AppendLine("  -- specified path: {0}", auditTask.ProjectFile);
			ModelPath sourcePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(auditTask.ProjectFile);
			var openOpts = new OpenOptions()
				{
					Audit = true,
					DetachFromCentralOption = DetachFromCentralOption.DoNotDetach
				};

			ModelPath docPath;

			/// the try block attempts to create a local file on the assumption that the project is workshared.
			/// If an exception is thrown when trying to create the local file, the catch block proceeds for a non-workshared project 
			/// todo: find a test for if(fileIsCentralFile) to replace the try block
			try
			{
				var segments = new Uri(auditTask.ProjectFile).Segments;
				String centralFileName = segments.Last();
				String localFilePath
					= RCronFileLocations.ResourcesDirectoryPath + RCronCanon.GetLocalFileName(centralFileName);
				docPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(localFilePath);

				WorksharingUtils.CreateNewLocal(sourcePath, docPath);
				log.AppendLine("  -- created local file " + localFilePath);

				isCentralFile = true;
				log.AppendLine("  -- project is workshared");
			}
			catch (Exception exc)
			{
				if (exc.Message.StartsWith("The model is not workshared")) //this indicates that the attempt to create a local file was unsucessful
				{
					docPath = sourcePath; //overwrite docPath to open the file directly, because it isn't a central file

					isCentralFile = false;
					log.AppendLine("  -- project is not workshared");
				}
				else
				{
					log.AppendLine("  !!  unknown exception");
					log.AppendLine("  !!  message: \"" + exc.Message + "\"");
					log.LogException(exc);
					return;
				}
			}

			var dbDoc = app.OpenDocumentFile(docPath, openOpts);

			if (isCentralFile)
			{
				var transactOpts = new TransactWithCentralOptions();
				var relinquishOpts = new RelinquishOptions(true); //creates an instance with all options set to true, so that everything will be relinquished
				var syncOpts = new SynchronizeWithCentralOptions();
				syncOpts.Comment = "Revit Cron Audit-Compact task run at " + DateTime.Now.ToString();
				syncOpts.Compact = true;
				syncOpts.SaveLocalAfter = false;
				syncOpts.SaveLocalBefore = false;
				syncOpts.SetRelinquishOptions(relinquishOpts); //documentation implies that this isn't necessary because default behavior already relinquishes everything
				dbDoc.SynchronizeWithCentral(transactOpts, syncOpts);
				log.AppendLine("  -- project compacted and synchronized with central");
				dbDoc.Close(false); //todo: leave open if more tasks are running on the document?
				String localPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(docPath);
				File.Delete(localPath); //delete the local file
				Directory.Delete(localPath.Remove(localPath.Length - 4, 4) + "_backup", true); //delete the backup files created with the local file
			}
			else
			{
				dbDoc.Close(); //save to original location
				log.AppendLine("  -- project saved");
			}

			return;
		}

		private static void RunExportTask(RCronExportTaskSpec exportTask, Document dbDoc)
		{
			log.AppendLine("\n-- specified path: {0}", dbDoc.PathName);

			//todo: create export task
			//dbDoc.Export

			return;
		}

		private static void RunETransmitTask(RCronETransmitTaskSpec eTransmitTask, Document dbDoc)
		{
			log.AppendLine("\n-- path: {0}", dbDoc.PathName);
			return;
		}

		private static void RunCommandTask(RCronCommandTaskSpec commandTask, Document dbDoc)
		{
			log.AppendLine("\n** running command task: {0}", dbDoc.PathName);
			return;
		}

		private static void RunPrintTask(RCronPrintTaskSpec printTask, Document dbDoc)
		{
			log.AppendLine("-- specified path: {0}", printTask.ProjectFile);
			log.AppendLine("-- loaded path: {0}", dbDoc.PathName);
			ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
					.OfClass(typeof(ViewSheetSet))
					.Where(s => s.Name.Equals(printTask.PrintSet))
					.FirstOrDefault() as ViewSheetSet;

			if (null == printSet)
			{
				log.AppendLine("  !! error: couldn't load printset {0}", printTask.PrintSet);
				return;
			}

			log.AppendLine("  ** printing {0} views", printSet.Views.Size.ToString());
			foreach (View v in printSet.Views)
			{
				log.AppendLine("  -- view: \"{0}\"", v.Name);
			}

			//verify outputDirectoryPath and OutputFileName and update as necessary
			String outputDirectoryPath = printTask.OutputDirectory + RCronCanon.TimeStamp + '\\';
			if (!Directory.Exists(outputDirectoryPath))
			{
				Directory.CreateDirectory(outputDirectoryPath);
			}
			else
			{
				if (File.Exists(outputDirectoryPath + printTask.OutputFileName)) //if the filename already exists, append an auto-incremented suffix to it
				{   //todo: move this stuff to RCronCanon
					Int32 version, highestVersion = 0;
					Regex suffixRegex = new Regex(@"(?<=\.)\d\d\d$");
					foreach (String f in Directory.GetFiles(outputDirectoryPath))
					{
						Match suffixMatch = suffixRegex.Match(f);
						if (suffixMatch.Success)
						{
							version = Int32.Parse(suffixMatch.Value);
							highestVersion = version > highestVersion ? version : highestVersion;
						}
					}
					highestVersion++;
					printTask.OutputFileName = suffixRegex.Replace(
							printTask.OutputFileName,
							String.Format("{0:D3}", highestVersion));
				}
			}

			using (Transaction t = new Transaction(dbDoc, "Run RCron Print Task"))
			{
				t.Start();

				PrintManager pm = dbDoc.PrintManager;
				pm.SelectNewPrintDriver("Bluebeam PDF"); //todo: read from RCronOptions
				pm.PrintSetup
						.InSession
						.PrintParameters
						.PaperSize = pm.PaperSizes
								.Cast<PaperSize>()
								.Where(p => "Letter" == p.Name) //todo: read from RCronOptions
								.FirstOrDefault();
				pm.PrintRange = PrintRange.Select;
				pm.ViewSheetSetting.CurrentViewSheetSet = printSet;
				pm.CombinedFile = true;
				pm.PrintToFileName = outputDirectoryPath + printTask.OutputFileName; //this value isn't used but SubmitPrint() thows an exception if it isn't set
				log.AppendLine("  ** Setting output destination");
				log.AppendLine("		-- PrintToFileName: {0}", pm.PrintToFileName);
				log.AppendLine("		-- OutputFileName: {0}", printTask.OutputFileName);
				log.AppendLine("		-- outputDirectoryPath: {0}", outputDirectoryPath);
				pm.SubmitPrint();
				t.Commit();
			}

			if (null == Thread.CurrentThread.Name)
			{
				Thread.CurrentThread.Name = "RotoGravure";
			}
			AutoResetEvent dialogHandlerWait = new AutoResetEvent(false);

			ThreadStart ts = new ThreadStart(() =>
			{
				BluebeamPrintDialogHandler.Save(
						dialogHandlerWait,
						outputDirectoryPath + printTask.OutputFileName);
			});
			Thread dialogHandlerThread = new Thread(ts);
			dialogHandlerThread.Name = "BluebeamDialogHandler";
			dialogHandlerThread.Start();
			try
			{
				dialogHandlerWait.WaitOne(10000); //give the handler 10 seconds to complete
			}
			catch (Exception)
			{
				log.AppendLine("  ++ error: dialog handler timed out");
			}
		}
	}
}
