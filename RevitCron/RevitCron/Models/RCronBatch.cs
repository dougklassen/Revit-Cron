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
		private List<RCronTask> batchTasks = new List<RCronTask>();

		[DataMember(Order = 0)]
		public IEnumerable<RCronTask> Tasks
		{
			get
			{
				return batchTasks.Select(t => t.GetBatchVersion());
			}
		}

		public void Add(RCronTask task)
		{
			batchTasks.Add(task);
		}
	}
}