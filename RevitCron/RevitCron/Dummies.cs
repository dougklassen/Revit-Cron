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
            TasksRepoUri = RCronFileLocations.AddInDirectoryPath + @"Resources\tasks.json"
        };

        public static readonly List<RCronTask> dummyTasks = new List<RCronTask>
        {
            new RCronTask()
            {
                Name = "Test Print Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronPrintTask()
                    {
                       TaskType = TaskType.Print,               
                       ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                       OutputDirectory = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        PrintSet = "test"
                    }
            },
            new RCronTask()
            {
                Name = "Test Export Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronExportTask()
                    {
                        TaskType = TaskType.Export,
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        PrintSet = "test",
                        ExportSetup = "test"
                    }
            },
            new RCronTask()
            {
                Name = "Test eTransmit Task",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronETransmitTask()
                    {
                        TaskType = TaskType.ETransmit,                        
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt"
                    }          
            },
            new RCronTask()
            {
                Name = "Print Task One",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronCommandTask()
                    {
                        TaskType = TaskType.Command,
                        ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        OutputDirectory = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
                        CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
                    }                
            },
        };
    }
}
