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
								Schedule = "0 0 * 0 0"	//should be run every jan 1st at midnight
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
			Assert.AreEqual(dumSchedule.Tasks.Count, batch.Tasks.Count);
			#endregion assert
		}

		[TestMethod]
		public void CanUpdateLastRun()
		{ }

		[TestMethod]
		public void PreventDuplicateScheduleTaskNames()
		{ }

		[TestMethod]
		public void PreventDuplicateBatchTaskNames()
		{ }

		[TestMethod]
		public void CanProcessScheduleMinutes()
		{ }
	}
}
