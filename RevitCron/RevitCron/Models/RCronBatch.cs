using System;
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
		/// A batch
		/// </summary>
		[DataMember(Order = 0)]
		private Dictionary<String, BatchTask> batchTasks;
		
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

		public void Add(String taskName, RCronTaskInfo task)
		{
			batchTasks.Add(taskName, new BatchTask(task));
		}

		//todo: private inner class to encapsulate task's name, TaskInfo, and outcome
		internal class BatchTask	//todo: check how scoping works for inner classes
		{
			RCronTaskInfo taskInfo;
			BatchTaskResult result;

			internal BatchTask(RCronTaskInfo info)
			{
				taskInfo = info;
				result = BatchTaskResult.NotRun;
			}
		}

		enum BatchTaskResult { NotRun, Suceeded, Failed }
	}	
}