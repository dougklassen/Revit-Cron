using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;

using DougKlassen.Revit.Cron.Rotogravure.Interface;

namespace DougKlassen.Revit.Cron.Rotogravure.StartUp
{
    public static class FileLocations
    {
        public static String AddInDirectoryPath;    //AddInDirectory is initialized at runtime
        public static String AssemblyName;
        public static String OptionsFilePath;
        public static readonly String ImperialTemplateDirectoryPath = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
        public static readonly String ResourceNameSpace = @"Rotogravure.Resources";
    }

    public class StartUpApp : IExternalApplication
    {
        StringBuilder logText = RCronLog.Instance.LogText;

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            //initialize AssemblyName using reflection
            FileLocations.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //initialize AddInDirectory. The addin should be stored in a directory named after the assembly
            FileLocations.AddInDirectoryPath = application.ControlledApplication.AllUsersAddinsLocation + "\\" + FileLocations.AssemblyName + "\\";

            application.ControlledApplication.ApplicationInitialized += OnApplicationInitialized;
            application.DialogBoxShowing += DialogEventHandler.OnDialogShowing;
            application.ApplicationClosing += application_ApplicationClosing;

            return Result.Succeeded;
        }

        void application_ApplicationClosing(object sender, Autodesk.Revit.UI.Events.ApplicationClosingEventArgs e)
        {
            LogWindow.Show(logText.ToString());
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
            logText.AppendLine("Rotogravure initialized");
            logText.AppendFormat("assembly: {0}\n", asm.Name);
            logText.AppendFormat("version: {0}\n", asm.Version);

            RotogravureOptions opts = new RotogravureOptionsJsonRepo(FileLocations.OptionsFilePath).GetRotogravureOptions();
            ICollection<RCronTask> tasks;
            try
            {
                tasks = new RCronTasksJsonRepo(opts.TasksRepoUri).GetRCronTasks();
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
                            RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo) task.TaskInfo;
                            ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
                                .OfClass(typeof(ViewSheetSet))
                                .Where(s => s.Name.Equals(printTaskInfo.PrintSet))
                                .FirstOrDefault() as ViewSheetSet;

                            if (null != printSet)
                            {
                                logText.AppendFormat("printing {0} views\n", printSet.Views.Size);
                                foreach(View v in printSet.Views)
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

        void LogException(Exception exception)
        {
            logText.AppendFormat("*** {0}\n", exception.GetType().ToString());
            logText.AppendLine(exception.Message);
            logText.AppendLine(exception.StackTrace);
            logText.AppendFormat("***\n");
        }
    }
}
