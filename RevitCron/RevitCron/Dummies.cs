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
                Name = "Print Task One",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronTaskInfo()
                    {
                       Task = TaskType.Print
                        //PrintSet = "test"
                    }
            },
            new RCronTask()
            {
                Name = "Print Task One",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronTaskInfo()
                    {
                        Task = TaskType.Export                        
                        //PrintSet = "test",
                        //ExportSetup = "test"
                    }
            },
            new RCronTask()
            {
                Name = "Print Task One",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronETransmitTask()
                    {
                        Task = TaskType.ETransmit,
                        OutputDirectory = @"C:\"
                    }          
            },
            new RCronTask()
            {
                Name = "Print Task One",
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                TaskInfo = new RCronCommandTask()
                    {
                        Task = TaskType.Command,
                        CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
                    }                
            },
        };
    }
}
