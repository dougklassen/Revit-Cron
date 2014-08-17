using DougKlassen.Revit.Cron;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	public class RCronSchedule
	{
		[DataMember(Order = 0)]
		public ICollection<RCronTask> Tasks	//todo: enforce unique names
		{
			get;
			set;
		}

		/// <summary>
		/// Obtain a batch of tasks to run based on the current time and last run date of tasks
		/// </summary>
		/// <returns>A batch to be used in running Revit tasks</returns>
		public RCronBatch GetRCronBatch()
		{
			RCronBatch batch = new RCronBatch();
			foreach (RCronTask task in Tasks)
			{
				if (task.IsDueToRun)
				{
					batch.Add(task);
				}
			}

			return batch;
		}

		/// <summary>
		/// Read last run time values from a batch
		/// </summary>
		/// <param name="batch">A batch with updated run times</param>
		/// <returns>The number of schedule tasks updated</returns>
		public Int32 UpdateLastRunFromBatch(RCronBatch batch)
		{
			Int32 numUpdates = 0;

			foreach (RCronTask t in batch.batchTasks)
			{
				IEnumerable<RCronTask> tasksToUpdate = Tasks.Where(u => u.Name == t.Name);
				foreach (RCronTask u in tasksToUpdate)
				{
					u.LastRun = t.LastRun;
					numUpdates++;
				}
			}

			return numUpdates;
		}
	}
}
