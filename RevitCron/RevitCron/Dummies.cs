using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron
{
    public static class Dummies
    {
        public static readonly RotogravureOptions dummyOpts = new RotogravureOptions()
        {
            TasksFileUri = new Uri(RCronFileLocations.AddInDirectoryPath + @"Resources\tasks.json"),
            LogDirectoryUri = new Uri(RCronFileLocations.AddInDirectoryPath + @"\Logs\")
        };

        public static readonly List<RCronTask> dummyTasks = new List<RCronTask>
        {
            new RCronTask()
            {
                Name = "Test Print Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronPrintTaskInfo()
                    {
                       TaskType = TaskType.Print,               
                       ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",  //todo: URI
                       OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
                       PrintSet = "test",
                       OutputFileName = "test.pdf"
                    }
            },
            new RCronTask()
            {
                Name = "Test Export Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronExportTaskInfo()
                    {
                        TaskType = TaskType.Export,
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
                        PrintSet = "test",
                        ExportSetup = "test"
                    }
            },
            new RCronTask()
            {
                Name = "Test eTransmit Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronETransmitTaskInfo()
                    {
                        TaskType = TaskType.ETransmit,                        
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\"
                    }          
            },
            new RCronTask()
            {
                Name = "Test Command Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronCommandTaskInfo()
                    {
                        TaskType = TaskType.Command,
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
                        CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
                    }                
            },
        };
    }
}
