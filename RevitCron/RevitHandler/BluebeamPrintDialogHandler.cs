using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Automation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	/// <summary>
	/// UI Automation for Bluebeam
	/// </summary>
	public static class BluebeamPrintDialogHandler
	{
		//todo: ensure hWnd doesn't get swithced from the print dialog
		private static RCronLog log = RCronLog.Instance;
		private static String saveFilePath;
		private static AutoResetEvent taskProcessingWaitHandle;
		private static IntPtr hWndSaveDialog = IntPtr.Zero;

		private const String SaveDialogCaption = "Save As";
		private const String SaveBtnText = "&Save";

		public static void Save(AutoResetEvent handle, String filePath)
		{
			taskProcessingWaitHandle = handle;
			saveFilePath = filePath;
			Thread.Sleep(3000); //give the window time to display
			log.AppendLine("  ** looking for save as dialog");
			WinApi.EnumWindows(EnumWindowsProcSaveAction, 0);
			Stopwatch rt = new Stopwatch();
			rt.Start();
			TimeSpan timeOut = new TimeSpan(0, 0, 30);
			while (IntPtr.Zero == hWndSaveDialog)	//wait while EnumWindowsProcSaveAction looks for window
			{
				Thread.Sleep(500);
				if (rt.Elapsed > timeOut)
				{
					throw new Exception("Couldn't find save dialog");
				}
			};

			System.IntPtr hWndChild;
			StringBuilder winTextSb = new StringBuilder(256);
			String winText;

			//set save location
			log.AppendLine("  ** looking for file location text box");
			List<IntPtr> hWndComboBoxes = new List<IntPtr>();
			hWndChild = WinApi.GetWindow(hWndSaveDialog, WinApi.GW_CHILD);
			do
			{
				WinApi.GetClassName(hWndChild, winTextSb, winTextSb.Capacity);
				winText = winTextSb.ToString();
				if (winText.Equals("ComboBoxEx32"))
				{
					hWndComboBoxes.Add(hWndChild);
				}
				hWndChild = WinApi.GetWindow(hWndChild, WinApi.GW_HWNDNEXT);
			} while (IntPtr.Zero != hWndChild);
			IntPtr hWndFileName = IntPtr.Zero;
			RECT rect = new RECT();
			Int32 minRect = Int32.MaxValue;
			for (int i = 0; i < hWndComboBoxes.Count; i++)
			{
				WinApi.GetWindowRect(hWndComboBoxes[i], ref rect);
				if (rect.Top < minRect)
				{
					hWndFileName = hWndComboBoxes[i];
					minRect = rect.Top;
				}
			}
			log.AppendLine("  -- file name textbox found: hWnd={0}");
			log.AppendLine("  ** setting text to \"{0}\"", saveFilePath);
			WinApi.SendMessage(hWndFileName, WinApi.WM_SETTEXT, 0, saveFilePath);

			//click Save As button
			log.AppendLine("  ** looking for Save As Button");
			winTextSb = new StringBuilder(256);
			hWndChild = WinApi.GetWindow(hWndSaveDialog, WinApi.GW_CHILD);
			do
			{
				WinApi.GetWindowText(hWndChild, winTextSb, winTextSb.Capacity);
				winText = winTextSb.ToString();
				if (SaveBtnText == winText)
				{
					log.AppendLine("  -- save button found: hWnd={0}, title=\"{1}\"", hWndChild, winText);
					log.AppendLine("  ** clicking button");
					WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
					WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONDOWN, 0, 0);
					WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONUP, 0, 0);
					WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
					//todo: prevent bluebeam from opening
					break;	//done with loop
				}
				hWndChild = WinApi.GetWindow(hWndChild, WinApi.GW_HWNDNEXT);
			} while (IntPtr.Zero != hWndChild);

			log.AppendLine("  ** print dialog handeled, releasing TaskProcessing to continue");
			log.LogThreadInfo();
			taskProcessingWaitHandle.Set(); //free TaskProcessingLogic to continue with other tasks
		}

		private static Boolean EnumWindowsProcSaveAction(IntPtr hWnd, Int32 lParam)
		{
			StringBuilder titleSb = new StringBuilder(256);
			WinApi.GetWindowText(hWnd, titleSb, titleSb.Capacity);
			String title = titleSb.ToString();

			if (SaveDialogCaption == title)
			{
				log.AppendLine("  -- target window found: hWnd={0}, title=\"{1}\"", hWnd, title);
				hWndSaveDialog = hWnd;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
