using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Repositories;

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
		/// Obtain a batch of tasks to run based on the specified time
		/// </summary>
		/// <param name="afterTime">
		/// Generate a batch for tasks after this time
		/// </param>
		/// <returns>A batch to be used in running Revit tasks</returns>
		public RCronBatch GetNextRCronBatch(DateTime afterTime)
		{
			RCronBatch batch = new RCronBatch();

			if (0 == Tasks.Count())	//if no tasks could be found, return the empty batch
			{
				return batch;
			}

			RCronOptions options = RCronOptionsJsonRepo.LoadOptions(RCronFileLocations.OptionsFilePath);
			batch.StartTime = Tasks.Min(t => t.NextRunTime(afterTime));	//find the time of the very next task scheduled to run after afterTime
			DateTime batchCutOff = batch.StartTime.Add(options.BatchSpan);	//calculate the window in which tasks will be grouped together
			List<RCronTask> batchTasks = Tasks
				.Where(t =>
					t.NextRunTime(afterTime) < batchCutOff)
					/////?filter out tasks that have completed early due to batching
					/////but were scheduled to run in the future. For these tasks
					/////last run will still be set to their scheduled (rather than actual) run time,
					/////which will be in the future
					//&& t.LastRun < earliestRunTime)
				.OrderBy(t => t.Priority).ToList();
			batch.EndTime = batchTasks.Max(t =>
				t.NextRunTime(afterTime)
				.Add(new TimeSpan(0,0,30)));
			batch.AddRange(batchTasks);	//feed the RCronTasks into a new RCronBatch

			return batch;
		}
	}
}
