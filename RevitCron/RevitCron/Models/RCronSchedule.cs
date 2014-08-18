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

			return batch;
		}
	}
}
