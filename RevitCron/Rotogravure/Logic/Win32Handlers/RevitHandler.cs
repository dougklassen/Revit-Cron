using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	/// <summary>
	/// UI Automation for Revit. This class should only be used within
	/// a Revit add-in
	/// </summary>
	public static class RevitHandler
	{
		/// <summary>
		/// Exit Revit
		/// </summary>
		public static void Exit()
		{
			IntPtr hWndRevit = GetRevitHandle();
			WinApi.User32.SendMessage(hWndRevit, WinApi.User32.WM_CLOSE, 0, 0);
		}

		private static IntPtr GetRevitHandle()
		{
			IntPtr hWndRevit = IntPtr.Zero;

			Process process = Process.GetCurrentProcess();
			hWndRevit = process.MainWindowHandle;

			return hWndRevit;
		}

		private static void ClickButton(IntPtr hWndChild)
		{
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONDOWN, 0, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONUP, 0, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
		}
	}
}
