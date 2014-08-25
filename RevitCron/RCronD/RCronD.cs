using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Forms;

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
		/// and queue a new batch. Value based on DaemonStatus enum
		/// </summary>
		private Int32 status;
		/// <summary>
		/// This is the run time of the last scheduled task of the most recently run batch.
		/// This must be tracked so that tasks that have come due before completion of
		/// the batch (and therefore the resumption of queueing) can be picked up for queueing
		/// </summary>
		private DateTime endOfLastBatch;
		/// <summary>
		/// The batch currently being run. This is a class level field so that RCronD can report on its current status.
		/// </summary>
		private RCronBatch batch;
		/// <summary>
		/// The wait handle used to pause the CheckSchedule callback while 
		/// </summary>
		private AutoResetEvent runBatchWait = new AutoResetEvent(false);

		/// <summary>
		/// The Singleton instance of the RCron Daemon
		/// </summary>
		public static RCronD Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Returns a summary of scheduled tasks and the queued batch
		/// </summary>
		public String StatusMessage
		{
			get
			{
				StringBuilder msg = new StringBuilder();
				msg.AppendLine("Scheduled Tasks:");
				msg.AppendLine("---");
				foreach (RCronTask t in Schedule.Tasks)
				{
					msg.AppendFormat("{0} - {1}\n", t.Name, t.TaskSpec.TaskType);
					msg.AppendFormat("Last Run: {0}\nNext Run: {1}\n", t.LastRun, t.NextRunTime());
					msg.AppendLine("---");
				}
				msg.AppendLine();

				if (Interlocked.Equals(status, (Int32)DaemonStatus.BatchQueued))
				{
					msg.AppendLine("Current Queue:");
					foreach (String taskName in batch.TaskSpecs.Keys)
					{
						msg.AppendFormat("{0} - {1}\n", taskName, batch.TaskSpecs[taskName].TaskType);
					}
					msg.AppendFormat("Start Time: {0}\nEnd Time: {1}\n", batch.StartTime, batch.EndTime);
				}
				else if(Interlocked.Equals(status, (Int32)DaemonStatus.RunningBatch))
				{
					msg.AppendLine("Batch is Running");
				}
				else
				{
					msg.AppendLine("No batch queued");
				}
				msg.AppendFormat("\nRunning on Thread {0}", Thread.CurrentThread.ManagedThreadId);
				return msg.ToString();
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
			status = (Int32)DaemonStatus.NoBatchQueued;
			endOfLastBatch = DateTime.Now;	//start looking for tasks from current time onwards
		}

		/// <summary>
		/// TimerCallback for main appcontext loop
		/// </summary>
		/// <param name="state"></param>
		public void CheckSchedule(Object state)
		{
			if (Interlocked.Equals(status, (Int32)DaemonStatus.NoBatchQueued))
			{
				batch = Schedule.GetNextRCronBatch(endOfLastBatch);
				if (0 == batch.TaskSpecs.Count()) return;	//if the batch is empty, no tasks were found, so don't try to run batch

				Interlocked.Exchange(ref status, (Int32)DaemonStatus.BatchQueued);	//signal that a batch is queued
				endOfLastBatch = batch.EndTime;	//move batch window forward

				RCronBatchJsonRepo batchRepo = new RCronBatchJsonRepo(new Uri(RCronFileLocations.BatchFilePath));
				batchRepo.PutRCronBatch(batch);

				TimeSpan timeTillRun = batch.StartTime - DateTime.Now;
				Timer runBatchTimer = new Timer();
				runBatchTimer.Interval =
					timeTillRun.TotalMilliseconds > 100 ? timeTillRun.TotalMilliseconds : 100;
				runBatchTimer.Elapsed += runBatchTimer_Elapsed;
				runBatchTimer.AutoReset = false;
				runBatchWait.Reset();	//reset the AutoResetEvent handle to wait
				runBatchTimer.Start();
				//todo: should this method return here and let all the cleanup happen in the timer callback?

				runBatchWait.WaitOne();	//wait till the callback signals to continue
				//todo: update last run times (maybe to endOfLastBatch?)
				//todo: record result of run
				batchRepo.Delete();	//cleanup the repo
				Interlocked.Exchange(ref status, (Int32)DaemonStatus.NoBatchQueued);
			}
		}

		private void runBatchTimer_Elapsed(Object sender, ElapsedEventArgs e)
		{
			Interlocked.Exchange(ref status, (Int32)DaemonStatus.RunningBatch);

			String msg;
			msg = String.Format("Started running task at {0}:{1}\nClick Ok to complete", DateTime.Now.Hour, DateTime.Now.Minute);
			System.Windows.Forms.MessageBox.Show(msg);
			//todo: start Revit here

			runBatchWait.Set();
		}
	}

	/// <summary>
	/// The possible states of RCronD, used to control the action of the CheckSchedule callback
	/// </summary>
	public enum DaemonStatus
	{
		/// <summary>
		/// No batch is currently queued
		/// </summary>
		NoBatchQueued,
		/// <summary>
		/// A batch is queued to run
		/// </summary>
		BatchQueued,
		/// <summary>
		/// A batch is running now
		/// </summary>
		RunningBatch
	}
}