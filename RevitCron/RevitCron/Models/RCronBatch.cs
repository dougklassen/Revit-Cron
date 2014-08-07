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
		[DataMember(Order = 0)]
		public ICollection<RCronTask> Tasks;	//todo: need to be reworked with a backing field but compatible with serialization
		
		public RCronBatch()
		{
			Tasks = new List<RCronTask>();
		}

		public void Add(RCronTask task)
		{
			Tasks.Add(task.GetBatchVersion());
		}
	}
}