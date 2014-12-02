using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using DougKlassen.Revit.Cron.Rotogravure.Interface;
using DougKlassen.Revit.Automation;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	public static class TaskProcessingLogic
	{
		private static RCronOptions options;
		private static RCronLog log;

		/// <summary>
		/// When Revit is started, check for a batch repo, load the batch, and run it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
		{
			{
				log = RCronLog.Instance;
				try
				{
					options = RCronOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
				}
				catch (Exception exc)
				{
					options = new RCronOptions();
					log.LogException(exc);
				}
			}

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

			Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
			UIApplication uiApp;
			Document dbDoc;
			UIDocument uiDoc;
			ModelPath docPath;
			OpenOptions openOpts;

			foreach (RCronTaskSpec	taskSpec in batch.TaskSpecs.Values)
			{
				try
				{
					switch (taskSpec.TaskType)
					{
						case RCronTaskType.Print:
							//todo: factor this into a method
							log.AppendLine("\n** running print task");
							log.AppendLine("-- specified path: {0}", taskSpec.ProjectFile);
							RCronPrintTaskSpec printTaskInfo = (RCronPrintTaskSpec)taskSpec;
							docPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(printTaskInfo.ProjectFile);
							openOpts = new OpenOptions();
							if (docPath.ServerPath)
							{
								openOpts.DetachFromCentralOption = DetachFromCentralOption.DetachAndPreserveWorksets; 
							}
							uiApp = new UIApplication(app);
							uiDoc = uiApp.OpenAndActivateDocument(docPath, openOpts, false);
							dbDoc = uiDoc.Document;
							//dbDoc = app.OpenDocumentFile(docPath, openOpts);
							log.AppendLine("-- loaded path: {0}", dbDoc.PathName);
							ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
									.OfClass(typeof(ViewSheetSet))
									.Where(s => s.Name.Equals(printTaskInfo.PrintSet))
									.FirstOrDefault() as ViewSheetSet;

							if (null != printSet)
							{
								log.AppendLine("  ** printing {0} views", printSet.Views.Size.ToString());
								foreach (View v in printSet.Views)
								{
									log.AppendLine("  -- view: \"{0}\"", v.Name);
								}

								//verify outputDirectoryPath and OutputFileName and update as necessary
								String outputDirectoryPath = printTaskInfo.OutputDirectory + RCronCanon.TimeStamp + '\\';
								if (!Directory.Exists(outputDirectoryPath))
								{
									Directory.CreateDirectory(outputDirectoryPath);
								}
								else
								{
									if (File.Exists(outputDirectoryPath + printTaskInfo.OutputFileName)) //if the filename already exists, append an auto-incremented suffix to it
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
										printTaskInfo.OutputFileName = suffixRegex.Replace(
												printTaskInfo.OutputFileName,
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
									//this value isn't used but SubmitPrint() thows an exception if it isn't set
									pm.PrintToFileName = outputDirectoryPath + printTaskInfo.OutputFileName;
									log.AppendLine("  ** Setting output destination");
									log.AppendLine("		-- PrintToFileName: {0}", pm.PrintToFileName);
									log.AppendLine("		-- OutputFileName: {0}", printTaskInfo.OutputFileName);
									log.AppendLine("		-- outputDirectoryPath: {0}", outputDirectoryPath);
									pm.SubmitPrint();
									t.Commit();
								}

								if (null == Thread.CurrentThread.Name)
								{
									Thread.CurrentThread.Name = "RotoGravure";
								}
								AutoResetEvent dialogHandlerWait = new AutoResetEvent(false);

								//BluebeamPrintDialogHandler.Save(
								//	externalTaskWaitHandle,
								//	outputDirectoryPath + printTaskInfo.OutputFileName);

								ThreadStart ts = new ThreadStart(() =>
									{
										BluebeamPrintDialogHandler.Save(
												dialogHandlerWait,
												outputDirectoryPath + printTaskInfo.OutputFileName);
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
							else
							{
								log.AppendLine("  !! error: couldn't load printset {0}", printTaskInfo.PrintSet);
							}
							//this won't work because we had to make the document active
							//dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
							break;
						case RCronTaskType.Export:
							dbDoc = app.OpenDocumentFile(taskSpec.ProjectFile);
							log.AppendLine("\n** running export task: {0}", dbDoc.PathName);
							dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
							break;
						case RCronTaskType.ETransmit:
							dbDoc = app.OpenDocumentFile(taskSpec.ProjectFile);
							log.AppendLine("\n** running eTransmit task: {0}", dbDoc.PathName);
							dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
							break;
						case RCronTaskType.Command:
							dbDoc = app.OpenDocumentFile(taskSpec.ProjectFile);
							log.AppendLine("\n** running command task: {0}", dbDoc.PathName);
							dbDoc.Close(false);	//todo: keep project open if running multiple tasks on it
							break;
						case RCronTaskType.AuditCompact:
							log.AppendLine("\n** running audit and compact task");
							log.AppendLine("-- specified path: {0}", taskSpec.ProjectFile);
							RCronAuditCompactTaskSpec acTaskInfo = (RCronAuditCompactTaskSpec)taskSpec;
							ModelPath sourcePath = ModelPathUtils.ConvertUserVisiblePathToModelPath(acTaskInfo.ProjectFile);
							openOpts = new OpenOptions();
							try	//todo: convert this to if(fileIsCentralFile)
							{
								FileInfo centralFile = new FileInfo(acTaskInfo.ProjectFile);
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
							dbDoc = app.OpenDocumentFile(docPath, openOpts);
							//todo: synchronize
							dbDoc.Close(false);
							break;
						case RCronTaskType.Test:
							TaskDialog.Show("Test", "Running test task");
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
			}	//end of task processing foreach

			//try		//todo: this moves to RCronD
			//{
			//	RCronSchedule schedule = RCronScheduleJsonRepo.LoadSchedule(options.ScheduleFileUri);
			//	//Int32 numUpdates = schedule.UpdateLastRunFromBatch(batch);
			//	RCronScheduleJsonRepo.WriteSchedule(options.ScheduleFileUri, schedule);
			//	//log.AppendLine("  ** updated {0} tasks in {1}", numUpdates, options.ScheduleFileUri.LocalPath);
			//}
			//catch (Exception exc)
			//{
			//	log.AppendLine("!! couldn't update schedule with last run times of task");
			//	log.LogException(exc);
			//}

			RevitHandler.Exit();
			log.AppendLine("** all tasks completed");
		}
	}
}
