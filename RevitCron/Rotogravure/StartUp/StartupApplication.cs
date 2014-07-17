using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

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
            String msg = String.Empty;
            msg += "Rotogravure initialized\n";
            msg += FileLocations.AssemblyName + "\n" + Assembly.GetExecutingAssembly().GetName().Version + '\n';

            RotogravureOptions opts = new RotogravureOptionsJsonRepo(FileLocations.OptionsFilePath).GetRotogravureOptions();
            ICollection<RCronTask> tasks;
            try
            {
                tasks = new RCronTasksJsonRepo(opts.TasksRepoUri).GetRCronTasks();
            }
            catch (Exception exception)
            {
                TaskDialog.Show("Error", "Could not load " + FileLocations.OptionsFilePath);
                throw exception;
            }

            msg += tasks.Count + " tasks found";

            TaskDialog.Show("Startup", msg);

            try
            {
                Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
                Document loadedDoc;
                Boolean saveModified = false;

                foreach (RCronTask task in tasks)
                {
                    loadedDoc = new Autodesk.Revit.ApplicationServices.Application().OpenDocumentFile(task.TaskInfo.ProjectFile);

                    switch (task.TaskInfo.TaskType)
                    {
                        case TaskType.Print:
                            TaskDialog.Show("Print task", "Printing...");
                            break;
                        case TaskType.Export:
                            break;
                        case TaskType.ETransmit:
                            break;
                        case TaskType.Command:
                            break;
                        default:
                            break;
                    }

                    loadedDoc.Close(saveModified);
                }
            }
            catch (Exception exception)
            {
                TaskDialog.Show("Exception", exception.Message + "\n" + exception.StackTrace);
                throw;
            }
        }
    }
}
