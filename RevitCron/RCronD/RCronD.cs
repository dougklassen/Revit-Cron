using DougKlassen.Revit.Cron.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		/// Whether a batch has been queued to run or not. If not, RCronD will generate and queue a new batch;
		/// </summary>
		private Boolean isBatchQueued = false;

		/// <summary>
		/// This is the run time of the last scheduled task of the most recently run batch.
		/// This must be tracked so that tasks that have come due before completion of the batch (and therefore the resumption
		/// of queueing) can be picked up for queueing
		/// </summary>
		private DateTime endOfLastBatch = DateTime.MinValue;

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
		}

		public void CheckSchedule(Object state)
		{
			System.Windows.Forms.MessageBox.Show("RCronD running");

			if (!isBatchQueued)
			{
				RCronBatch batch = Schedule.GetNextRCronBatch(endOfLastBatch);
			}
			
		}

		public void QueueBatch(RCronBatch batch)
		{
			//set callback to run at run time
		}
	}
}