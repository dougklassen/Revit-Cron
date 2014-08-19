﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	public class RCronBatch
	{
		/// <summary>
		/// A batch of tasks to be run by Revit upon startup
		/// </summary>
		private Dictionary<String, BatchTask> batchTasks;
		
		[DataMember(Order=0)]
		public IEnumerable<RCronTaskSpec> TaskSpecs
		{
			get
			{
				return batchTasks.Values.Select(s => s.taskSpec);
			}
		}

		public RCronBatch()
		{
			batchTasks = new Dictionary<String, BatchTask>();
		}

		public void Add(RCronTask task)
		{
			Add(task.Name, task.TaskInfo);
		}

		public void AddRange(IEnumerable<RCronTask> tasks)
		{
			foreach (RCronTask t in tasks)
			{
				Add(t.Name, t.TaskInfo);
			}
		}

		public void Add(String taskName, RCronTaskSpec taskSpec)
		{
			batchTasks.Add(taskName, new BatchTask(taskSpec));
		}

		internal class BatchTask	//todo: check how scoping works for inner classes
		{
			internal RCronTaskSpec taskSpec;
			internal BatchTaskResult result;

			internal BatchTask(RCronTaskSpec info)
			{
				taskSpec = info;
				result = BatchTaskResult.NotRun;
			}
		}

		public enum BatchTaskResult { NotRun, Suceeded, Failed }
	}	
}