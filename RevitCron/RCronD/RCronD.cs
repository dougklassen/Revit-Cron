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
		private static RCronD instance = new RCronD();

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

			
		}
	}
}