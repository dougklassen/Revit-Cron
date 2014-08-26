using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
	}
}
