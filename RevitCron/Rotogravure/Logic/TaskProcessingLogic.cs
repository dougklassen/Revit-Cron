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
        private static StringBuilder logText = RCronLog.Instance.LogText;

        public static void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
            logText.AppendLine("Rotogravure initialized");
            logText.AppendFormat("assembly: {0}\n", asm.Name);
            logText.AppendFormat("version: {0}\n", asm.Version);

            RotogravureOptions opts;
            ICollection<RCronTask> tasks;
            try
            {
                opts = new RotogravureOptionsJsonRepo(RCronFileLocations.OptionsFilePath).GetRotogravureOptions();
                tasks = new RCronTasksJsonRepo(opts.TasksFileUri).GetRCronTasks();
            }
            catch (Exception exception)
            {
                LogException(exception);
                LogWindow.Show(logText.ToString());
                throw exception; //can't continue if the TasksRepo can't be loaded
            }

            logText.AppendFormat("{0} tasks found\n", tasks.Count);

            Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
            Document dbDoc;
            Boolean saveModified = false;

            foreach (RCronTask task in tasks)
            {
                try
                {
                    dbDoc = app.OpenDocumentFile(task.TaskInfo.ProjectFile);
                    logText.AppendLine("---");

                    switch (task.TaskInfo.TaskType)
                    {
                        case TaskType.Print:
                            logText.AppendFormat("print task: {0}\n", dbDoc.PathName);
                            RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo)task.TaskInfo;
                            ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
                                .OfClass(typeof(ViewSheetSet))
                                .Where(s => s.Name.Equals(printTaskInfo.PrintSet))
                                .FirstOrDefault() as ViewSheetSet;

                            if (null != printSet)
                            {
                                logText.AppendFormat("printing {0} views\n", printSet.Views.Size);
                                foreach (View v in printSet.Views)
                                {
                                    logText.AppendFormat("view found: {0}\n", v.Name);
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
                                logText.AppendFormat("error: couldn't load printset {0}\n", printTaskInfo.PrintSet);
                            }

                            break;
                        case TaskType.Export:
                            logText.AppendFormat("export task: {0}\n", dbDoc.PathName);
                            break;
                        case TaskType.ETransmit:
                            logText.AppendFormat("eTransmit task: {0}\n", dbDoc.PathName);
                            break;
                        case TaskType.Command:
                            logText.AppendFormat("command task: {0}\n", dbDoc.PathName);
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

            LogWindow.Show(logText.ToString());
        }

        static private void LogException(Exception exception) //todo: move to strongly typed log object in rcron
        {
            logText.AppendFormat("*** {0}\n", exception.GetType().ToString());
            logText.AppendLine(exception.Message);
            logText.AppendLine(exception.StackTrace);
            logText.AppendFormat("***\n");
        }
    }
}
