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

			CreateNotifyIcon();

			daemon = RCronD.Instance;
			daemon.Schedule = schedule;

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
			//timer.
		}

		private void exitItem_Click(object sender, EventArgs e)
		{
			ExitThread();
		}
	
		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			MessageBox.Show("RCronD is running");
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
			base.ExitThreadCore();
		}

		private Icon GetEmbeddedIconResource(String iconName)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream mrs = asm.GetManifestResourceStream("DougKlassen.Revit.Cron.Daemon.Resources." + iconName);
			return new Icon(mrs);
		}

		private void LoadRCronSettings()
		{
			try
			{
				options = RCronOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
			}
			catch (Exception exc)	//if options couldn't be loaded, quit
			{
				Console.WriteLine("Couldn't load options from {0}", RCronFileLocations.OptionsFilePath);
				Console.WriteLine(exc.Message);
				Console.Write(exc.StackTrace);
				ExitThread();
			}

			try //todo: why isn't an exception thrown here
			{
				schedule = RCronScheduleJsonRepo.LoadSchedule(options.ScheduleFileUri);
				if (null == schedule)
				{
					Console.WriteLine("Schedule is null");
				}
			}
			catch (Exception exc)	//if options couldn't be loaded, quit
			{
				Console.WriteLine("Couldn't load schedule from {0}", options.ScheduleFileUri.LocalPath);
				Console.WriteLine(exc.Message);
				Console.Write(exc.StackTrace);
				ExitThread();
			}
		}

		private void CreateNotifyIcon()
		{
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
			notifyIcon.Click += notifyIcon_Click;
		}
	}
}
