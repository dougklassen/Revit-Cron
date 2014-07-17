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
            new rCronPrintTask()
            {
                Name = "Print Task One",
                Task = TaskType.Print,
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                PrintSet = "test"
            },
            new rCronExportTask()
            {
                Name = "Print Task One",
                Task = TaskType.Print,
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                PrintSet = "test",
                ExportSetup = "test"
            },
            new rCronETransmitTask()
            {
                Name = "Print Task One",
                Task = TaskType.Print,
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                OutputDirectory = @"C:\"
            },
            new rCronCommandTask()
            {
                Name = "Print Task One",
                Task = TaskType.Print,
                LastRun = new DateTime(),
                Schedule = "0 0 * * *",
                CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
            },
        };
    }
}
