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
		/// A collection of tasks to run specified for processing by Rotogravure
		/// todo:
		/// </summary>
		[DataMember(Order = 0)]
		public Dictionary<String, RCronTaskSpec> TaskSpecs;

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
			TaskSpecs = new Dictionary<String, RCronTaskSpec>();
		}

		public void Add(String taskName, RCronTaskSpec taskSpec)
		{
			TaskSpecs.Add(taskName, taskSpec);
		}

		public void Add(RCronTask task)
		{
			TaskSpecs.Add(task.Name, task.TaskSpec);
		}

		public void AddTasks(IEnumerable<RCronTask> tasks)
		{
			foreach (RCronTask t in tasks)
			{
				TaskSpecs.Add(t.Name, t.TaskSpec);
			}
		}
	}	
}