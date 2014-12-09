using Autodesk.Revit.DB;
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
		static RCronLog log = RCronLog.Instance;
		static RCronOptions options;
		static RevitHandler revitHandler = RevitHandler.Instance;

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

			foreach (RCronTaskSpec taskSpec in batch.TaskSpecs.Values)
			{
				try
				{
					switch (taskSpec.TaskType)
					{
						case RCronTaskType.Print:
							log.AppendLine("\n** running print task");
							RunPrintTask(taskSpec as RCronPrintTaskSpec, app);
							break;
						case RCronTaskType.Export:
							log.AppendLine("\n** running export task");
							RunExportTask(taskSpec as RCronExportTaskSpec, app);
							break;
						case RCronTaskType.ETransmit:
							log.AppendLine("\n** running eTransmit task");
							RunETransmitTask(taskSpec as RCronETransmitTaskSpec, app);
							break;
						case RCronTaskType.Command:
							log.AppendLine("\n** running command task");
							RunCommandTask(taskSpec as RCronCommandTaskSpec, app);
							break;
						case RCronTaskType.AuditCompact:
							log.AppendLine("\n** running audit and compact task");
							RunAuditCompactTask(taskSpec as RCronAuditCompactTaskSpec, app);
							break;
						case RCronTaskType.Test:
							//TaskDialog.Show("Test", "Running test task");
							log.AppendLine("\n** running test task");
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

			revitHandler.CloseActive(); //in case a doc is active
			revitHandler.Exit(synchronize: false, save: false); //close Revit without saving or synchronizing any open documents
			log.AppendLine("** all tasks completed");
		}

		private static void RunAuditCompactTask(RCronAuditCompactTaskSpec auditTask, Application app)
		{
			log.AppendLine("-- specified path: {0}", auditTask.ProjectFile);
			ModelPath sourcePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(auditTask.ProjectFile);
			var openOpts = new OpenOptions();
			ModelPath docPath;
			try	//todo: convert this to if(fileIsCentralFile)
			{
				FileInfo centralFile = new FileInfo(auditTask.ProjectFile);
				String centralFileName = centralFile.Name;
				String localFilePath
					= RCronFileLocations.ResourcesDirectoryPath + RCronCanon.GetLocalFileName(centralFileName);
				docPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(localFilePath);
				WorksharingUtils.CreateNewLocal(docPath, sourcePath);
				log.AppendLine("  -- project is workshared");
			}
			catch (Autodesk.Revit.Exceptions.ArgumentException)
			{
				log.AppendLine("  -- project is not workshared");
				docPath = sourcePath;
			}
			if (docPath.ServerPath)
			{
				openOpts.DetachFromCentralOption = DetachFromCentralOption.DoNotDetach;
				openOpts.Audit = true;
				//todo: create new local
			}
			var dbDoc = app.OpenDocumentFile(docPath, openOpts);
			//todo: synchronize
			dbDoc.Close(false); //todo: leave open if more tasks are running on the document

			return;
		}

		private static void RunExportTask(RCronExportTaskSpec exportTask, Application app)
		{
			var dbDoc = app.OpenDocumentFile(exportTask.ProjectFile);
			log.AppendLine("\n-- specified path: {0}", dbDoc.PathName);
			dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
			return;
		}

		private static void RunETransmitTask(RCronETransmitTaskSpec eTransmitTask, Application app)
		{
			var dbDoc = app.OpenDocumentFile(eTransmitTask.ProjectFile);
			log.AppendLine("\n-- path: {0}", dbDoc.PathName);
			dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
			return;
		}

		private static void RunCommandTask(RCronCommandTaskSpec commandTask, Application app)
		{
			var dbDoc = app.OpenDocumentFile(commandTask.ProjectFile);
			log.AppendLine("\n** running command task: {0}", dbDoc.PathName);
			dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
			return;
		}

		private static void RunPrintTask(RCronPrintTaskSpec printTask, Application app)
		{
			log.AppendLine("-- specified path: {0}", printTask.ProjectFile);

			var docPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(printTask.ProjectFile);
			var openOpts = new OpenOptions();
			if (docPath.ServerPath)
			{
				openOpts.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets;
			}

			var uiApp = new UIApplication(app);
			//todo: check that the document isn't open and active already before opening
			//todo: test what happens when the doc is already open
			var uiDoc = uiApp.OpenAndActivateDocument(docPath, openOpts, false); //We need to open and activate the doc at the ui level or the Bluebeam print driver will crash when run on VDI. Can't use dbDoc = app.OpenDocumentFile(docPath, openOpts);
			var dbDoc = uiDoc.Document;
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
			revitHandler.CloseActive();
			//We can't close the document with dbDoc.Close(false) because we had to make it active. It will be left open
		}
	}
}
