﻿using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Timer = System.Threading.Timer;
using System.Windows.Forms;

namespace DougKlassen.Revit.Cron.Daemon
{
	public class RCronDAppContext : ApplicationContext
	{
		Container components;
		NotifyIcon notifyIcon;

		RCronOptions options;
		RCronSchedule schedule;
		RCronD daemon;
		Timer timer;

		public RCronDAppContext()
		{
			InitializeContext();
		}

		private void InitializeContext()
		{
			//todo: logging for RCronD
			Console.WriteLine("Initializing RCronD");

			LoadRCronSettings();
			daemon = RCronD.Instance;
			daemon.Schedule = schedule;

			components = new Container();
			notifyIcon = new System.Windows.Forms.NotifyIcon(components)
			{
				ContextMenuStrip = new ContextMenuStrip(),
				Icon = GetEmbeddedIconResource("rcron.ico"),
				Text = "RCronD",
				Visible = true
			};

			notifyIcon.ContextMenuStrip.Items.Add("&Pause", null, pauseItem_Click);
			notifyIcon.ContextMenuStrip.Items.Add("&Exit", null, exitItem_Click);
			notifyIcon.DoubleClick += notifyIcon_DoubleClick;

			timer = new Timer(daemon.CheckSchedule, null, TimeSpan.Zero, options.PollingPeriod);

			//todo: watch for changes to options.json and schedule.json
		}

		void notifyIcon_Click(object sender, EventArgs e)
		{
			MethodInfo methodInfo = typeof(NotifyIcon).GetMethod(
				"ShowContextMenu",
				BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo.Invoke(notifyIcon, null);
		}
		
		private void pauseItem_Click(object sender, EventArgs e)
		{
			//todo:	not implemented
		}


		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			MessageBox.Show(daemon.StatusMessage, "RCronD Status");
		}

		private void exitItem_Click(object sender, EventArgs e)
		{
			ExitThread();
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing && null != components)
			{
				components.Dispose();
				timer.Dispose();
			}
		}

		protected override void ExitThreadCore()
		{
			if (null != notifyIcon)
			{
				notifyIcon.Visible = false; 
			}
			IRCronBatchRepo repo = new RCronBatchJsonRepo(new Uri(RCronFileLocations.BatchFilePath));	//todo: setup dependency injection
			repo.Delete();	//flush the repo (for file repos, delete the file)
			base.ExitThreadCore();
		}

		private Icon GetEmbeddedIconResource(String iconName)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream mrs = asm.GetManifestResourceStream("DougKlassen.Revit.Cron.Daemon.Resources." + iconName);
			return new Icon(mrs);
		}

		/// <summary>
		/// Load options and schedule from file repo
		/// </summary>
		private void LoadRCronSettings()
		{
			try
			{
				options = RCronOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
			}
			catch (Exception exc)	//if options couldn't be loaded, quit
			{
				MessageBox.Show("Exception during options load");
				//ExitThread();
			}

			try
			{
				schedule = RCronScheduleJsonRepo.LoadSchedule(options.ScheduleFileUri);
				if (null == schedule)
				{
					MessageBox.Show("Schedule is null");
				}
			}
			catch (Exception exc)
			{
				MessageBox.Show("Exception during schedule load");

				//ExitThread();	//todo: what does this do, program doesn't end
				//possibly exiting wrong thread
			}
		}
	}
}
