using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DougKlassen.Revit.Cron.Daemon
{
	public class RCronDAppContext : ApplicationContext
	{
		Container components;
		NotifyIcon notifyIcon;

		public RCronDAppContext()
		{
			InitializeContext();
		}

		private void InitializeContext()
		{
			Console.WriteLine("Initializing RCronD");

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

		void notifyIcon_Click(object sender, EventArgs e)
		{
			MethodInfo methodInfo = typeof(NotifyIcon).GetMethod(
				"ShowContextMenu",
				BindingFlags.Instance | BindingFlags.NonPublic);
			methodInfo.Invoke(notifyIcon, null);
		}
		
		private void pauseItem_Click(object sender, EventArgs e)
		{
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
			}
		}

		protected override void ExitThreadCore()
		{
			notifyIcon.Visible = false;
			base.ExitThreadCore();
		}

		private Icon GetEmbeddedIconResource(String iconName)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream mrs = asm.GetManifestResourceStream("DougKlassen.Revit.Cron.Daemon.Resources." + iconName);
			return new Icon(mrs);
		}
	}
}
