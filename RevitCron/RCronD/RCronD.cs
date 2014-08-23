using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
//using Timer = System.Timers.Timer;

namespace DougKlassen.Revit.Cron.Daemon
{
	/// <summary>
	/// Singleton Class providing callbacks to run RevitCron
	/// </summary>
	public class RCronD
	{
		/// <summary>
		/// The Singleton instance
		/// </summary>
		private static RCronD instance = new RCronD();

		/// <summary>
		/// Whether a batch has been queued to run or not. If not, RCronD will generate
		/// and queue a new batch;
		/// </summary>
		private Boolean isBatchQueued;

		/// <summary>
		/// This is the run time of the last scheduled task of the most recently run batch.
		/// This must be tracked so that tasks that have come due before completion of
		/// the batch (and therefore the resumption of queueing) can be picked up for queueing
		/// </summary>
		private DateTime endOfLastBatch;

		public static RCronD Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// The schedule being run by the daemon
		/// </summary>
		public RCronSchedule Schedule
		{
			get;
			set;
		}

		private RCronD()
		{
			Schedule = null;	//Schedule must be set to initialize RCronD
			isBatchQueued = false;
			endOfLastBatch = DateTime.Now;	//start looking for tasks from current time onwards
		}

		/// <summary>
		/// TimerCallback for main appcontext loop
		/// </summary>
		/// <param name="state"></param>
		public void CheckSchedule(Object state)
		{
			if (!isBatchQueued)
			{
				RCronBatch batch = Schedule.GetNextRCronBatch(endOfLastBatch);
				if (0 == batch.TaskSpecs.Count())	//if the batch is empty, no tasks were found, so don't try to run batch
				{
					System.Windows.Forms.MessageBox.Show("No tasks found");
					return;
				}
				isBatchQueued = true;
				endOfLastBatch = batch.EndTime;	//move batch window forward

				RCronBatchJsonRepo batchRepo = new RCronBatchJsonRepo(new Uri(RCronFileLocations.BatchFilePath));
				batchRepo.PutRCronBatch(batch);

				Console.WriteLine("running batch");
				TimeSpan timeTillRun = batch.StartTime - DateTime.Now;
				Timer runBatchTimer = new Timer();
				runBatchTimer.Interval =
					timeTillRun.TotalMilliseconds > 100 ? timeTillRun.TotalMilliseconds : 100;
				runBatchTimer.Elapsed += runBatchTimer_Elapsed;
				runBatchTimer.AutoReset = false;
				//runBatchTimer.SynchronizingObject = ;
				runBatchTimer.Start();

				//todo: wait for Revit to close/batch to finish
				//todo: record result of run
				batchRepo.Delete();	//cleanup the repo
				//isBatchQueued = false;
			}
		}

		private void runBatchTimer_Elapsed(Object sender, ElapsedEventArgs e)
		{
			System.Windows.Forms.MessageBox.Show("Pretending to run something");
			isBatchQueued = false;
			//todo: startup Revit here
			}
	}
}