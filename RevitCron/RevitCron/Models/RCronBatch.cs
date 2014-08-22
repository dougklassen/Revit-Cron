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
		private Dictionary<String, BatchTask> batchTasks;
		
		/// <summary>
		/// A collection of tasks to run specified for processing by Rotogravure
		/// </summary>
		[DataMember(Order=0)]
		public IEnumerable<RCronTaskSpec> TaskSpecs
		{
			get
			{
				return batchTasks.Values.Select(s => s.taskSpec);
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
			batchTasks = new Dictionary<String, BatchTask>();
		}

		public void Add(String taskName, RCronTaskSpec taskSpec)
		{
			batchTasks.Add(taskName, new BatchTask(taskSpec));
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
			internal RCronTaskSpec taskSpec;
			internal BatchTaskResult result;

			internal BatchTask(RCronTaskSpec spec)
			{
				taskSpec = spec;
				result = BatchTaskResult.NotRun;
			}
		}

		public enum BatchTaskResult { NotRun, Suceeded, Failed }
	}	
}