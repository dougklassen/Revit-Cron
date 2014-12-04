using Autodesk.Revit.UI.Events;
using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Forms;

namespace DougKlassen.Revit.Automation
{
	/// <summary>
	/// A class supporting Revit UI automation
	/// </summary>
	public sealed class RevitHandler
	{
		/// <summary>
		/// Field for specifying response behavior to 
		/// </summary>
		private DialogResponse DialogHandler { get; set; }

		/// <summary>
		/// Singleton static initializer
		/// </summary>
		private static readonly RevitHandler instance = new RevitHandler();

		/// <summary>
		/// Accessor for singleton instance
		/// </summary>
		public static RevitHandler Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Private default constructor
		/// </summary>
		private RevitHandler()
		{
			DialogHandler = new DialogResponse();
		}

		/// <summary>
		/// Get a reference to the Revit main window
		/// </summary>
		public static AutomationElement GetRevitWindow()
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

		/// <summary>
		/// Get a reference to the "AdApplicationButton" which holds the Revit program menu
		/// </summary>
		public static AutomationElement GetAdAppButton()
		{
			var revit = GetRevitWindow();
			if (revit == null)
			{
				return null;
			}
			else
			{
				AutomationElement button;
				PropertyCondition cond = new PropertyCondition(AutomationElement.NameProperty, "AdApplicationButton");
				button = revit.FindFirst(TreeScope.Children, cond);
				return button;
			}
		}

		/// <summary>
		/// Get a handle to Revit using Win32
		/// </summary>
		/// <returns></returns>
		private static IntPtr GetRevitHandle()
		{
			IntPtr hWndRevit = IntPtr.Zero;

			Process process = Process.GetCurrentProcess();
			hWndRevit = process.MainWindowHandle;

			return hWndRevit;
		}

		/// <summary>
		/// Exit Revit 
		/// </summary>
		public void Exit(Boolean synchronize = false, Boolean save = false)
		{
			if (save)
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 6); //yes
			}
			else
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 7); //no
			}
			IntPtr hWndRevit = GetRevitHandle();
			WinApi.SendMessage(hWndRevit, WinApi.WM_CLOSE, 0, 0);
			DialogHandler.ClearOverrides();
		}

		/// <summary>
		/// Send a button click to the specified button element
		/// </summary>
		/// <param name="hWndChild"></param>
		private static void ClickButton(IntPtr hWndChild)
		{
			WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
			WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONDOWN, 0, 0);
			WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONUP, 0, 0);
			WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
		}

		/// <summary>
		/// Carry out dialog handling according to current dialog handling settings
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