using DougKlassen.Revit.Cron.Models;

using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Cron
{
	public static class Dummies
	{
		public static readonly RCronOptions dummyOpts = new RCronOptions()
		{
			BatchFileUri = new Uri(RCronFileLocations.BatchFilePath),
			ScheduleFileUri = new Uri(RCronFileLocations.ScheduleFilePath),
			LogDirectoryUri = new Uri(RCronFileLocations.LogDirectoryPath),
			PollingPeriod = new TimeSpan(0, 0, 7)
		};

		public static readonly List<RCronTask> dummyTasks = new List<RCronTask>
		{
      new RCronTask()
      {
        Name = "Test Print Task 1",
        LastRun = new DateTime(),
        Schedule = "0 0 * * *",
				Priority = 2,
        TaskInfo = new RCronPrintTaskInfo()
        {
          TaskType = TaskType.Print,
          ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",  //todo: URI
          OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
          PrintSet = "test1",
          OutputFileName = "test1.pdf"
        }
      },
			new RCronTask()
      {
        Name = "Test Print Task 2",
        LastRun = new DateTime(),
        Schedule = "0 0 * * *",
				Priority = 2,
        TaskInfo = new RCronPrintTaskInfo()
        {
          TaskType = TaskType.Print,
          ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",  //todo: URI
          OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
          PrintSet = "test2",
          OutputFileName = "test2.pdf"
        }
      },
      new RCronTask()
      {
        Name = "Test Export Task",
        LastRun = new DateTime(),
        Schedule = "0 0 * * *",
				Priority = 1,
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
				Priority = 3,
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
				Priority = 3,
        TaskInfo = new RCronCommandTaskInfo()
        {
          TaskType = TaskType.Command,
          ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
          OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
          CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
        }                
      }
    };

		public static RCronSchedule dummySchedule;

		public static RCronBatch dummyBatch;

		static Dummies()
		{
			dummySchedule = new RCronSchedule();
			dummyBatch = new RCronBatch();
			foreach (RCronTask t in dummyTasks)
			{
				dummyBatch.Add(t);
			}
		}
	}
}