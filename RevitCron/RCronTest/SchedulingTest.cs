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
			RCronTask task = new RCronTask()
			{
				Name = "task",
				Schedule = "0 12 * * *"	//run every day at 1200
			};
			DateTime runAtTime = new DateTime(1999, 1, 1, 11, 30, 0);	//simulate a run at 1130
			DateTime calculatedNextRun;
			#endregion arrange

			#region act
			calculatedNextRun = task.NextRunTime(runAtTime);
			#endregion act

			#region assert
			DateTime expectedRunTime = new DateTime(1999, 1, 1, 12, 0, 0);
			Assert.AreEqual(expectedRunTime, calculatedNextRun);
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
