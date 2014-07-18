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

namespace DougKlassen.Revit.Cron.Rotogravure.StartUp
{
    public static class FileLocations
    {
        //AddInDirectory is initialized at runtime
        public static String AddInDirectoryPath;
        public static String AssemblyName;
        public static String OptionsFilePath;
        public static readonly String ImperialTemplateDirectoryPath = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
        public static readonly String ResourceNameSpace = @"Rotogravure.Resources";
    }

    public class StartUpApp : IExternalApplication
    {
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            //initialize AssemblyName using reflection
            FileLocations.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //initialize AddInDirectory. The addin should be stored in a directory named after the assembly
            FileLocations.AddInDirectoryPath = application.ControlledApplication.AllUsersAddinsLocation + "\\" + FileLocations.AssemblyName + "\\";
            FileLocations.OptionsFilePath = FileLocations.AddInDirectoryPath + @"Resources\ini.json";

            application.ControlledApplication.ApplicationInitialized += OnApplicationInitialized;

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            StringBuilder resultString = new StringBuilder();
            AssemblyName asm = Assembly.GetExecutingAssembly().GetName();
            resultString.AppendLine("Rotogravure initialized");
            resultString.AppendFormat("assembly: {0}\n", asm.Name);
            resultString.AppendFormat("version: {0}\n", asm.Version);

            RotogravureOptions opts = new RotogravureOptionsJsonRepo(FileLocations.OptionsFilePath).GetRotogravureOptions();
            ICollection<RCronTask> tasks;
            try
            {
                tasks = new RCronTasksJsonRepo(opts.TasksRepoUri).GetRCronTasks();
            }
            catch (Exception exception)
            {
                TaskDialog exceptionDlg = new TaskDialog("Couldn't Load Tasks");
                exceptionDlg.MainContent = exception.Message;
                exceptionDlg.ExpandedContent = exception.StackTrace;
                exceptionDlg.Show();
                throw exception;
            }

            resultString.AppendFormat("{0} tasks found\n", tasks.Count);

            try
            {
                Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
                Document loadedDoc;
                Boolean saveModified = false;

                foreach (RCronTask task in tasks)
                {
                    loadedDoc = app.OpenDocumentFile(task.TaskInfo.ProjectFile);
                    resultString.AppendLine("---");

                    switch (task.TaskInfo.TaskType)
                    {
                        case TaskType.Print:
                            resultString.AppendFormat("print task: {0}\n", loadedDoc.PathName);
                            RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo) task.TaskInfo;
                            ViewSheetSet printSet = new FilteredElementCollector(loadedDoc)
                                .OfClass(typeof(ViewSheetSet))
                                .Where(s => s.Name.Equals(printTaskInfo.PrintSet))
                                .FirstOrDefault() as ViewSheetSet;

                            if (null != printSet)
                            {
                                resultString.AppendFormat("printing {0} views\n", printSet.Views.Size);
                                foreach(View v in printSet.Views)
                                {
                                    resultString.AppendFormat("view found: {0}\n", v.Name);
                                }
                                loadedDoc.Print(printSet.Views);
                            }
                            else
                            {
                                resultString.AppendFormat("error: couldn't load printset {0}\n", printTaskInfo.PrintSet);
                            }

                            break;
                        case TaskType.Export:
                            resultString.AppendFormat("export task: {0}\n", loadedDoc.PathName);
                            break;
                        case TaskType.ETransmit:
                            resultString.AppendFormat("eTransmit task: {0}\n", loadedDoc.PathName);
                            break;
                        case TaskType.Command:
                            resultString.AppendFormat("command task: {0}\n", loadedDoc.PathName);
                            break;
                        default:
                            break;
                    }

                    loadedDoc.Close(saveModified);
                }
            }
            catch (Exception exception)
            {
                TaskDialog exceptionDlg = new TaskDialog("Couldn't Execute Tasks");
                exceptionDlg.MainContent = exception.Message;
                exceptionDlg.ExpandedContent = exception.StackTrace;
                exceptionDlg.Show();

                TaskDialog dlg = new TaskDialog("Result");
                dlg.MainContent = "Rotogravure Tasks Completed";
                dlg.ExpandedContent = resultString.ToString();
                dlg.Show();

                throw exception;
            }

            TaskDialog resultDlg = new TaskDialog("Result");
            resultDlg.MainContent = "Rotogravure Tasks Completed";
            resultDlg.ExpandedContent = resultString.ToString();
            resultDlg.Show();
        }
    }
}
