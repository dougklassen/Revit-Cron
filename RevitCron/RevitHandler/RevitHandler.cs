using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace DougKlassen.Revit.Automation
{
	/// <summary>
	/// A class supporting Revit UI automation
	/// </summary>
	public static class RevitHandler
	{
		/// <summary>
		/// A reference to the Revit main window
		/// </summary>
		public static AutomationElement RevitWindow
		{
			get
			{
				AutomationElement revit = null;

				PropertyCondition contentPropCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
				var windows = AutomationElement.RootElement.FindAll(TreeScope.Children, contentPropCondition);

				foreach (AutomationElement window in windows)
				{
					String name = window.GetCurrentPropertyValue(AutomationElement.NameProperty).ToString();
					if (name.StartsWith("Autodesk Revit "))
					{
						revit = window;
					}
				}

				return revit;
			}
		}

		/// <summary>
		/// A reference to the "AdApplicationButton" which holds the Revit program menu
		/// </summary>
		public static AutomationElement AdAppButton
		{
			get
			{
				var revit = RevitWindow;
				if (revit == null)
				{
					return null;
				}
				AutomationElement button;
				PropertyCondition cond = new PropertyCondition(AutomationElement.NameProperty, "AdApplicationButton");
				button = RevitWindow.FindFirst(TreeScope.Children, cond);
				return button;
			}
		}

		/// <summary>
		/// Exit Revit
		/// </summary>
		public static void Exit()
		{
			IntPtr hWndRevit = GetRevitHandle();
			WinApi.SendMessage(hWndRevit, WinApi.WM_CLOSE, 0, 0);
		}
	
		private static IntPtr GetRevitHandle()
		{
			IntPtr hWndRevit = IntPtr.Zero;

			Process process = Process.GetCurrentProcess();
			hWndRevit = process.MainWindowHandle;

			return hWndRevit;
		}

		/// <summary>
		/// When Revit is started, check for a batch repo, load the batch, and run it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void OnDialogShowing(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
		{
			var args = e as TaskDialogShowingEventArgs;
			if (args != null)
			{
				var msg = string.Format("HelpId: {0}\nDialogId: {1}\nMessage: {2}", args.HelpId, args.DialogId, args.Message);
				MessageBox.Show(msg, "Dialog Showing");
			}
			else
			{
				MessageBox.Show("Event Type: " + e.GetType().ToString(), "Dialog Showing");
			}
		}
	}
}