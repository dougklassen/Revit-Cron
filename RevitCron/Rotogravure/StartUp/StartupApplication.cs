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
        //AddInDirectory is initialized at runtime
        public static String AddInDirectoryPath;
        public static String AssemblyName;
        public static String OptionsFilePath;
        public static readonly String ImperialTemplateDirectoryPath = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
        public static readonly String ResourceNameSpace = @"Rotogravure.Resources";
    }

    public class StartUpApp : IExternalApplication
    {
        StringBuilder resultString;

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
            resultString = new StringBuilder(String.Empty);
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
                LogException(exception);
                LogWindow.Show(resultString.ToString());
                throw exception; //can't continue if the TasksRepo can't be loaded
            }

            resultString.AppendFormat("{0} tasks found\n", tasks.Count);

            Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
            Document dbDoc;
            Boolean saveModified = false;

            foreach (RCronTask task in tasks)
            {
                try
                {
                    dbDoc = app.OpenDocumentFile(task.TaskInfo.ProjectFile);
                    resultString.AppendLine("---");

                    switch (task.TaskInfo.TaskType)
                    {
                        case TaskType.Print:
                            resultString.AppendFormat("print task: {0}\n", dbDoc.PathName);
                            RCronPrintTaskInfo printTaskInfo = (RCronPrintTaskInfo) task.TaskInfo;
                            ViewSheetSet printSet = new FilteredElementCollector(dbDoc)
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

                                PrintManager pm = dbDoc.PrintManager;
                                pm.SelectNewPrintDriver("Bluebeam PDF");
                                pm.PrintSetup.InSession.PrintParameters.PaperSize = pm.PaperSizes.Cast<PaperSize>().Where(p => "Letter" == p.Name).FirstOrDefault();
                                pm.PrintRange = PrintRange.Select;
                                pm.ViewSheetSetting.CurrentViewSheetSet = printSet;
                                pm.CombinedFile = true;
                                pm.SubmitPrint();
                                //dbDoc.Print(printSet.Views);
                            }
                            else
                            {
                                resultString.AppendFormat("error: couldn't load printset {0}\n", printTaskInfo.PrintSet);
                            }

                            break;
                        case TaskType.Export:
                            resultString.AppendFormat("export task: {0}\n", dbDoc.PathName);
                            break;
                        case TaskType.ETransmit:
                            resultString.AppendFormat("eTransmit task: {0}\n", dbDoc.PathName);
                            break;
                        case TaskType.Command:
                            resultString.AppendFormat("command task: {0}\n", dbDoc.PathName);
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

            LogWindow.Show(resultString.ToString());
        }

        void LogException(Exception exception)
        {
            resultString.AppendFormat("*** {0}\n", exception.GetType().ToString());
            resultString.AppendLine(exception.Message);
            resultString.AppendLine(exception.StackTrace);
            resultString.AppendFormat("***\n");
        }
    }
}
