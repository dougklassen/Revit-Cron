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

		/// <summary>
		/// Add an RCronTaskSpec to the batch
		/// </summary>
		/// <param name="taskName">The name used to identify the task</param>
		/// <param name="taskSpec">The RCronTaskSpec defining the task</param>
		public void Add(String taskName, RCronTaskSpec taskSpec)
		{
			TaskSpecs.Add(taskName, taskSpec);
		}

		/// <summary>
		/// Add an RCronTask to the batch
		/// </summary>
		/// <param name="task">The task to add to the batch</param>
		public void Add(RCronTask task)
		{
			TaskSpecs.Add(task.Name, task.TaskSpec);
		}

		/// <summary>
		/// Add a collection of tasks to the batch
		/// </summary>
		/// <param name="tasks">A collection of tasks to add to the batch</param>
		public void AddTasks(IEnumerable<RCronTask> tasks)
		{
			foreach (RCronTask t in tasks)
			{
				TaskSpecs.Add(t.Name, t.TaskSpec);
			}
		}
	}	
}