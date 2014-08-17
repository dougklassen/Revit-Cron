using Moq;
using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RCronTest
{
	[TestClass]
	public class SchedulingTest
	{
		[TestMethod]
		public void CanGetBatch()
		{
			#region arrange
			//Mock<RCronSchedule> mockSchedule = new Mock<RCronSchedule>();
			//mockSchedule.Setup(s => s.Tasks).Returns<IEnumerable<RCronTask>>(t =>
			//	{
			//		List<RCronTask> dumTasks = new List<RCronTask>();
			//		for (int i = 0; i < 5; i++)
			//		{
			//			dumTasks.Add(new RCronTask() { Name = "Task" + i });
			//		}
			//		return dumTasks;
			//	});
			List<RCronTask> dumTasks = new List<RCronTask>();
			for (int i = 0; i < 5; i++)
			{
				dumTasks.Add(new RCronTask()
					{
						Name = "Task" + i,
						Schedule = "* * * * *",	//should be run every minute
						LastRun = DateTime.MinValue
					});
			}
			RCronSchedule dumSchedule = new RCronSchedule()
			{
				Tasks = dumTasks
			};
			#endregion arrange

			#region act
			RCronBatch batch = dumSchedule.GetRCronBatch();
			#endregion act

			#region assert
			Assert.AreEqual(dumSchedule.Tasks.Count, batch.batchTasks.Count);
			#endregion assert
		}

		[TestMethod]
		public void CanUpdateLastRun()
		{ }

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PreventDuplicatBatchTaskNamesOnAdd()
		{
			#region arrange
			RCronBatch dumBatch = new RCronBatch();

			for (int i = 0; i < 5; i++)
			{
				dumBatch.Add(new RCronTask()
				{
					Name = "Task" + i
				});
			}
			#endregion arrange

			#region act
			dumBatch.Add(new RCronTask()
				{
					Name = "Task0"	//shouldn't be able to add a Task when a matching name already exists
				});
			#endregion act
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PreventDuplicateBatchTaskNamesOnSet()
		{
			#region arrange

			RCronBatch dumBatch = new RCronBatch();
			List<RCronTask> dumTasks = new List<RCronTask>()
			{
				new RCronTask()	{	Name = "task"	},
				new RCronTask() { Name = "task" }
			};
			#endregion arrange

			#region act
			dumBatch.batchTasks = dumTasks;
			#endregion act
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void PreventDuplicateScheduleTaskNamesOnSet()
		{
			#region arrange

			RCronSchedule dumSchedule = new RCronSchedule();
			List<RCronTask> dumTasks = new List<RCronTask>()
			{
				new RCronTask()	{	Name = "task"	},
				new RCronTask() { Name = "task" }
			};
			#endregion arrange

			#region act
			dumSchedule.Tasks = dumTasks;
			#endregion act
		}

		[TestMethod]	//todo: what if the task completes a little after the beginning of the current minute?
		public void CanDetermineIsDueToRunEveryMinute()
		{
			#region arrange
			RCronTask task = new RCronTask()
			{
				Schedule = "* * * * *",	//run every minute
				LastRun = DateTime.Now.Subtract(new TimeSpan(0, 1, 1))	//LastRun was more than a minute ago
			};
			Boolean shouldRun;
			#endregion arrange

			#region act
			shouldRun = task.IsDueToRun;
			#endregion act

			#region assert
			Assert.IsTrue(shouldRun);
			#endregion assert
		}

		[TestMethod] //todo: what if the task completes a little after the hour it was scheduled to run?
		public void CanDetermineIsDueToRunHourly()
		{
			#region arrange
			RCronTask task = new RCronTask()
			{
				Schedule = "0 * * * *",	//run at the beginning of every hour
				LastRun = new DateTime(
					DateTime.Now.Year,
					DateTime.Now.Month,
					DateTime.Now.Day,
					DateTime.Now.Hour,
					59,
					59)
					.Subtract(new TimeSpan(1, 0, 0))	//last run was in the final second of the previous hour
			};
			Boolean shouldRun;
			#endregion arrange

			#region act
			shouldRun = task.IsDueToRun;
			#endregion act

			#region assert
			Assert.IsTrue(shouldRun);
			#endregion assert
		}	

		[TestMethod]	//todo: what if the task completes a few minutes after midnight?
		public void CanDetermineIsDueToRunDaily()
		{
			#region arrange
			RCronTask task = new RCronTask()
			{
				Schedule = "0 0 * * *",	//run every morning at midnight
				LastRun = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0)
					.Subtract(new TimeSpan(1, 0, 0, 1))	//last run was in the final second of the previous day
			};
			Boolean shouldRun;
			#endregion arrange

			#region act
			shouldRun = task.IsDueToRun;
			#endregion act

			#region assert
			Assert.IsTrue(shouldRun);
			#endregion assert
		}
	}
}
