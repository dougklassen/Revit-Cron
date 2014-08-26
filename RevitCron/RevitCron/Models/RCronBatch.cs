using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	[KnownType(typeof(RCronPrintTaskSpec))]
	[KnownType(typeof(RCronExportTaskSpec))]
	[KnownType(typeof(RCronETransmitTaskSpec))]
	[KnownType(typeof(RCronCommandTaskSpec))]
	[KnownType(typeof(RCronTestTaskSpec))]
	public class RCronBatch
	{
		/// <summary>
		/// A batch of tasks to be run by Revit upon startup
		/// </summary>
		private List<BatchTask> batchTasks;
		
		/// <summary>
		/// A collection of tasks to run specified for processing by Rotogravure
		/// </summary>
		[DataMember(Order=0)]
		public Dictionary<String, RCronTaskSpec> TaskSpecs
		{
			get
			{
				Dictionary<String, RCronTaskSpec> taskSpecs = new Dictionary<string, RCronTaskSpec>();
				foreach (BatchTask bT in batchTasks)
				{
					taskSpecs.Add(bT.taskName, bT.taskSpec);
				}
				return taskSpecs;
			}
			set
			{
				batchTasks = new List<BatchTask>();
				foreach (String key in value.Keys)
				{
					batchTasks.Add(new BatchTask(key, value[key]));
				}
			}
		}

		/// <summary>
		/// The scheduled runtime of the batch, set by the scheduled run-time of the first task in the batch
		/// </summary>
		[DataMember(Order = 1)]
		public DateTime StartTime;

		/// <summary>
		/// The scheduled (vs actual) end time of the batch, set by the scheduled run-time of the last task in the batch
		/// </summary>
		[DataMember(Order = 2)]
		public DateTime EndTime;

		/// <summary>
		/// An empty RCronBatch. StartTime and EndTime are for the use of the class that makes use of RCronBatch
		/// </summary>
		public RCronBatch()
		{
			batchTasks = new List<BatchTask>();
		}

		public void Add(String taskName, RCronTaskSpec taskSpec)
		{
			batchTasks.Add(new BatchTask(taskName, taskSpec));
		}

		public void Add(RCronTask task)
		{
			Add(task.Name, task.TaskSpec);
		}

		public void AddRange(IEnumerable<RCronTask> tasks)
		{
			foreach (RCronTask t in tasks)
			{
				Add(t.Name, t.TaskSpec);
			}
		}

		internal class BatchTask	//todo: check how scoping works for inner classes
		{
			internal String taskName;
			internal RCronTaskSpec taskSpec;
			internal BatchTaskResult result;

			internal BatchTask(String name, RCronTaskSpec spec)
			{
				taskName = name;
				taskSpec = spec;
				result = BatchTaskResult.NotRun;
			}
		}

		public enum BatchTaskResult { NotRun, Suceeded, Failed }
	}	
}