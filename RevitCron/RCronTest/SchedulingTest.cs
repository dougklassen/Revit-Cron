using Moq;
using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RCronTest
{
	[TestClass]
	public class SchedulingTest
	{
		[TestMethod]
		public void CanGetNextRunTime()
		{
			#region arrange
			var task1 = new RCronTask()
			{
				Name = "task1",
				Schedule = "0 12 * * *"	//run every day at 1200
			};
			var task2 = new RCronTask()
			{
				Name = "task2",
				Schedule = "0 12 * 6 *" //run every day in June at 1200
			};

			var runAtTime1 = new DateTime(1999, 1, 1, 11, 30, 0);	//simulate a run at 1130
			var runAtTime2 = new DateTime(1999, 5, 1, 11, 30, 0); //simulate a run at 1130 on May 1st
			DateTime calculatedNextRun1, calculatedNextRun2;
			#endregion arrange

			#region act
			calculatedNextRun1 = task1.NextRunTime(runAtTime1);
			calculatedNextRun2 = task2.NextRunTime(runAtTime2);
			#endregion act

			#region assert
			var expectedRunTime1 = new DateTime(1999, 1, 1, 12, 0, 0);
			var expectedRunTime2 = new DateTime(1999, 6, 1, 12, 0, 0);
			Assert.AreEqual(expectedRunTime1, calculatedNextRun1);
			Assert.AreEqual(expectedRunTime2, calculatedNextRun2);
			#endregion assert
		}

		[TestMethod]
		public void CanGetBatch()
		{
			#region arrange
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
			RCronBatch batch = dumSchedule.GetNextRCronBatch(DateTime.Now);
			#endregion act

			#region assert
			Assert.AreEqual(dumSchedule.Tasks.Count, batch.TaskSpecs.Count());
			#endregion assert
		}

		[TestMethod]
		public void CanUpdateLastRun()
		{ 
			throw new NotImplementedException();
#region arrange
#endregion arrange

#region act
#endregion act

#region assert
#endregion assert
		}

		[TestMethod]
		public void CanAddTaskToBatch()
		{
			#region arrange
			RCronBatch dumBatch = new RCronBatch();
			RCronTask dumTask = new RCronTask()
			{
				Name = "dumTask"
			};
			#endregion arrange

			#region act
			dumBatch.Add(dumTask);
			#endregion act

			#region assert
			Assert.IsTrue(1 == dumBatch.TaskSpecs.Count());
			#endregion assert
		}

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
			dumBatch.AddTasks(dumTasks);
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
	}
}
