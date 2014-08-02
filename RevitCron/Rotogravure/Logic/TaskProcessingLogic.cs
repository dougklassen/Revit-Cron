﻿using Autodesk.Revit.DB;

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
		private static RotogravureOptions options;
		private static RCronLog log;

		public static void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
		{
			{
				log = RCronLog.Instance;
				try
				{
					options = RotogravureOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
				}
				catch (Exception exc)
				{
					options = new RotogravureOptions();
					log.LogException(exc);
				}
			}

			AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
			log.AppendLine("+++ {0} Rotogravure initialized", DateTime.Now);
			log.AppendLine("assembly: {0}", asm.Name);
			log.AppendLine("version: {0}", asm.Version.ToString());

			ICollection<RCronTask> tasks;
			try
			{
				tasks = RCronTasksJsonRepo.LoadTasks(options.TasksFileUri);
			}
			catch (Exception exception)
			{
				log.LogException(exception);
				LogWindow.Show(log);
				throw exception; //can't continue if the TasksRepo can't be loaded
			}

			log.AppendLine("+++ {0} tasks found", tasks.Count);

			Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
			Document dbDoc;
			Boolean saveModified = false;

			foreach (RCronTask task in tasks)
			{
				try
				{
					dbDoc = app.OpenDocumentFile(task.TaskInfo.ProjectFile);
					log.AppendLine("---");

					switch (task.TaskInfo.TaskType)
					{
						case TaskType.Print:
							log.AppendLine("print task: {0}", dbDoc.PathName);
							RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo)task.TaskInfo;
							ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
									.OfClass(typeof(ViewSheetSet))
									.Where(s => s.Name.Equals(printTaskInfo.PrintSet))
									.FirstOrDefault() as ViewSheetSet;

							if (null != printSet)
							{
								log.AppendLine("--printing {0} views", printSet.Views.Size.ToString());
								foreach (View v in printSet.Views)
								{
									log.AppendLine("  view: \"{0}\"", v.Name);
								}

								PrintManager pm = dbDoc.PrintManager;
								pm.SelectNewPrintDriver("Bluebeam PDF"); //todo: read from RotogravureOptions
								pm.PrintSetup
										.InSession
										.PrintParameters
										.PaperSize = pm.PaperSizes
												.Cast<PaperSize>()
												.Where(p => "Letter" == p.Name) //todo: read from RotogravureOptions
												.FirstOrDefault();
								pm.PrintRange = PrintRange.Select;
								pm.ViewSheetSetting.CurrentViewSheetSet = printSet;
								pm.CombinedFile = true;
								pm.SubmitPrint();

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

								Thread.CurrentThread.Name = "RotoGravure thread";
								AutoResetEvent externalTaskWaitHandle = new AutoResetEvent(false);
								BluebeamPrintDialogHandler.Save(
									externalTaskWaitHandle,
									outputDirectoryPath + printTaskInfo.OutputFileName);
								////launch the dialog handler in a new thread and wait until it completes work
								//AutoResetEvent externalTaskWaitHandle = new AutoResetEvent(false);
								//ThreadStart ts = new ThreadStart(() =>
								//	{
								//		BluebeamPrintDialogHandler.Save(
								//				externalTaskWaitHandle,
								//				outputDirectoryPath + printTaskInfo.OutputFileName);
								//	});
								//Thread dialogHandlerThread = new Thread(ts)
								//	{
								//		Name = "BluebeamDialogHandler"
								//	};
								//dialogHandlerThread.Start();
								////externalTaskWaitHandle.WaitOne();
							}
							else
							{
								log.AppendLine("error: couldn't load printset {0}", printTaskInfo.PrintSet);
							}

							break;
						case TaskType.Export:
							log.AppendLine("export task: {0}", dbDoc.PathName);
							break;
						case TaskType.ETransmit:
							log.AppendLine("eTransmit task: {0}", dbDoc.PathName);
							break;
						case TaskType.Command:
							log.AppendLine("command task: {0}", dbDoc.PathName);
							break;
						default:
							break;
					}

					dbDoc.Close(saveModified);
				}
				catch (Exception exception)
				{
					log.LogException(exception);
				}
			}
		}
	}
}
