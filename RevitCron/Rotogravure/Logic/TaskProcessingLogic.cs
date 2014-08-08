using Autodesk.Revit.DB;

using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using DougKlassen.Revit.Cron.Rotogravure.Interface;

using System;
using System.Collections.Generic;
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

				batchRepo = new RCronBatchJsonRepo(options.BatchFileUri);
				batch = batchRepo.GetRCronBatch();
			}
			catch (Exception exception)
			{
				log.AppendLine("!! Couldn't load batch from {0}", options.BatchFileUri.LocalPath);
				log.LogException(exception);
				LogWindow.Show(log);
				throw exception; //can't continue if the TasksRepo can't be loaded
			}

			log.AppendLine("  -- number of tasks found: {0}", batch.Tasks.Count());

			Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
			Document dbDoc;
			Boolean saveModified = false;

			foreach (RCronTask task in batch.Tasks)
			{
				try
				{
					dbDoc = app.OpenDocumentFile(task.TaskInfo.ProjectFile);

					switch (task.TaskInfo.TaskType)
					{
						case TaskType.Print:
							log.AppendLine("\n** running print task: {0}", dbDoc.PathName);
							RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo)task.TaskInfo;
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

								using (Transaction t = new Transaction(dbDoc, "Set printset"))
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
									pm.SubmitPrint();
									t.Commit();
								}

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

								if (null == Thread.CurrentThread.Name)
								{
									Thread.CurrentThread.Name = "RotoGravure"; 
								}
								AutoResetEvent externalTaskWaitHandle = new AutoResetEvent(false);

								//BluebeamPrintDialogHandler.Save(
								//	externalTaskWaitHandle,
								//	outputDirectoryPath + printTaskInfo.OutputFileName);

								ThreadStart ts = new ThreadStart(() =>
									{
										BluebeamPrintDialogHandler.Save(
												externalTaskWaitHandle,
												outputDirectoryPath + printTaskInfo.OutputFileName);
									});
								Thread dialogHandlerThread = new Thread(ts);
								dialogHandlerThread.Name = "BluebeamDialogHandler";
								dialogHandlerThread.Start();
								try
								{
									externalTaskWaitHandle.WaitOne(10000); //give the handler 10 seconds to complete
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

							break;
						case TaskType.Export:
							log.AppendLine("\n** running export task: {0}", dbDoc.PathName);
							break;
						case TaskType.ETransmit:
							log.AppendLine("\n** running eTransmit task: {0}", dbDoc.PathName);
							break;
						case TaskType.Command:
							log.AppendLine("\n** running command task: {0}", dbDoc.PathName);
							break;
						default:
							break;
					}

					dbDoc.Close(saveModified);	//todo: keep project open if running multiple tasks on it

					task.LastRun = DateTime.Now; //todo: set this to scheduled run time rather than completion time
				}
				catch (Exception exception)
				{
					log.LogException(exception);
				}

				try
				{
					RCronSchedule schedule = RCronScheduleJsonRepo.LoadSchedule(options.ScheduleFileUri);
					Int32 numUpdates = schedule.UpdateLastRunFromBatch(batch);
					RCronScheduleJsonRepo.WriteSchedule(options.ScheduleFileUri, schedule);
					log.AppendLine("  ** updated {0} tasks in {1}", numUpdates, options.ScheduleFileUri.LocalPath);
				}
				catch (Exception exc)
				{
					log.AppendLine("!! couldn't update schedule with last run times of task");
					log.LogException(exc);
				}
				log.AppendLine("** deleting {0}", RCronFileLocations.BatchFilePath);
				batchRepo.Delete();
			}
		}
	}
}
