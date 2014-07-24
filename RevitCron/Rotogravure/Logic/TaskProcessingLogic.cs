using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using DougKlassen.Revit.Cron.Rotogravure;
using DougKlassen.Revit.Cron.Rotogravure.Interface;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public static class TaskProcessingLogic
    {
        private static RotogravureOptions options;
        private static RCronLog log;

        public static void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            options = RotogravureOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
            try
            {
                log = RCronLogFileRepo.LoadLog(options.LogFileUri);
            }
            catch (Exception)
            {
                log = RCronLog.Instance;    //if the log file couldn't be loaded, get a new RCronLog
            }

            AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
            log.AppendLine("Rotogravure initialized");
            log.AppendLine("assembly: {0}", asm.Name);
            log.AppendLine("version: {0}", asm.Version.ToString());

            ICollection<RCronTask> tasks;
            try
            {
                tasks = RCronTasksJsonRepo.LoadTasks(options.TasksFileUri);
            }
            catch (Exception exception)
            {
                LogException(exception);
                LogWindow.Show(log);
                throw exception; //can't continue if the TasksRepo can't be loaded
            }

            log.AppendLine("{0} tasks found", tasks.Count);

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
                            log.AppendLine("print task: {0}\n", dbDoc.PathName);
                            RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo)task.TaskInfo;
                            ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
                                .OfClass(typeof(ViewSheetSet))
                                .Where(s => s.Name.Equals(printTaskInfo.PrintSet))
                                .FirstOrDefault() as ViewSheetSet;

                            if (null != printSet)
                            {
                                log.AppendLine("printing {0} views\n", printSet.Views.Size.ToString());
                                foreach (View v in printSet.Views)
                                {
                                    log.AppendLine("view found: {0}\n", v.Name);
                                }

                                PrintManager pm = dbDoc.PrintManager;
                                pm.SelectNewPrintDriver("Bluebeam PDF");
                                pm.PrintSetup.InSession.PrintParameters.PaperSize = pm.PaperSizes.Cast<PaperSize>().Where(p => "Letter" == p.Name).FirstOrDefault();
                                pm.PrintRange = PrintRange.Select;
                                pm.ViewSheetSetting.CurrentViewSheetSet = printSet;
                                pm.CombinedFile = true;
                                pm.SubmitPrint();
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
                    LogException(exception);
                }
            }

            LogWindow.Show(log);
        }

        static private void LogException(Exception exception) //todo: move to strongly typed log object in rcron
        {
            log.AppendLine("*** {0}", exception.GetType());
            log.AppendLine(exception.Message);
            log.AppendLine(exception.StackTrace);
            log.AppendLine("***");
        }
    }
}
