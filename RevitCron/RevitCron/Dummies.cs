using DougKlassen.Revit.Cron.Models;

using System;
using System.Collections.Generic;

namespace DougKlassen.Revit.Cron
{
	public static class Dummies
	{
		public static readonly RCronOptions dummyOpts = new RCronOptions()
		{
			BatchFileUri = RCronUris.BatchFileUri,
			ScheduleFileUri = new Uri(RCronFileLocations.ScheduleFilePath),
			LogDirectoryUri = new Uri(RCronFileLocations.LogDirectoryPath),
			LocalFileDirectoryUri = new Uri(RCronFileLocations.AddInDirectoryPath),
			PollingPeriod = new TimeSpan(0, 0, 7),
			BatchSpan = new TimeSpan(0, 10, 0)
		};

		public static readonly List<RCronTask> dummyTestTasks = new List<RCronTask>
		{
			new RCronTask()
			{
				Name = "Test_1min",
				LastRun = DateTime.MinValue,
				Schedule = "* * * * *",	//run every 15 minutes
				Priority = 1,
				TaskSpec = new RCronTestTaskSpec()
			},
			new RCronTask()
			{
				Name = "Test_15min",
				LastRun = DateTime.MinValue,
				Schedule = "*/15 * * * *",	//run every 15 minutes
				Priority = 1,
				TaskSpec = new RCronTestTaskSpec()
			},
			new RCronTask()
			{
				Name = "Test_30min",
				LastRun = DateTime.MinValue,
				Schedule = "0,30 * * * *",	//run every half hour
				Priority = 1,
				TaskSpec = new RCronTestTaskSpec()
			}
		};

		public static readonly List<RCronTask> dummyTasks = new List<RCronTask>
		{
      new RCronTask()
      {
        Name = "Test Print Task 1",
        LastRun = new DateTime(),
        Schedule = "0 0 * * *",
				Priority = 2,
        TaskSpec = new RCronPrintTaskSpec()
        {
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
        TaskSpec = new RCronPrintTaskSpec()
        {
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
        TaskSpec = new RCronExportTaskSpec()
        {
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
        TaskSpec = new RCronETransmitTaskSpec()
        {
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
        TaskSpec = new RCronCommandTaskSpec()
        {
          ProjectFile = @"C:\Users\dklassen\Source\Repos\Revit-Cron\playpen\playpen.rvt",
          OutputDirectory = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Test Output\",
          CommandName = "DougKlassen.Revit.Perfect.Commands.RenameFamiliesCommand"
        }                
      }
    };

		public static RCronSchedule testSchedule;
		public static RCronSchedule dummySchedule;
		public static RCronBatch dummyBatch;

		static Dummies()
		{
			dummySchedule = new RCronSchedule() { Tasks = dummyTasks };
			testSchedule = new RCronSchedule() { Tasks = dummyTestTasks };
			dummyBatch = new RCronBatch();
			foreach (RCronTask t in dummyTasks)
			{
				dummyBatch.Add(t);
			}
		}
	}
}