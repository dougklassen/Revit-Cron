using DougKlassen.Revit.Cron.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	public static class BluebeamPrintDialogHandler
	{
		private static RCronLog log = RCronLog.Instance;
		private static String saveFilePath;
		private static AutoResetEvent taskProcessingWaitHandle;
		private static IntPtr hWndSaveDialog = IntPtr.Zero;

		private const String SaveDialogCaption = "Save As";
		private const String SaveBtnText = "&Save";

		public static void Save(AutoResetEvent handle, String filePath)
		{
			Thread currThrd = Thread.CurrentThread;
			System.Windows.Forms.MessageBox.Show(String.Format(
				"In Save method\nthread: {0}\nId: {1}\napplication context: {2}",
				currThrd.Name,
				currThrd.ManagedThreadId.ToString(),
				AppDomain.CurrentDomain.FriendlyName));

			taskProcessingWaitHandle = handle;
			saveFilePath = filePath;
			Thread.Sleep(3000); //give the window time to display
			WinApi.User32.EnumWindows(EnumWindowsProcSaveAction, 0);
			Stopwatch rt = new Stopwatch();
			rt.Start();
			TimeSpan timeOut = new TimeSpan(0, 0, 30);
			while(IntPtr.Zero == hWndSaveDialog)
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
			List<IntPtr> hWndComboBoxes = new List<IntPtr>();
			hWndChild = WinApi.User32.GetWindow(hWndSaveDialog, WinApi.User32.GW_CHILD);
			do
			{
				WinApi.User32.GetClassName(hWndChild, winTextSb, winTextSb.Capacity);
				winText = winTextSb.ToString();
				if (winText.Equals("ComboBoxEx32"))
				{
					hWndComboBoxes.Add(hWndChild);
				}
				hWndChild = WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT);
			} while (IntPtr.Zero != hWndChild);
			IntPtr hWndFileName = IntPtr.Zero;
			RECT rect = new RECT();
			Int32 minRect = Int32.MaxValue;
			for (int i = 0; i < hWndComboBoxes.Count; i++)
			{
				WinApi.User32.GetWindowRect(hWndComboBoxes[i], ref rect);
				if (rect.Top < minRect)
				{
					hWndFileName = hWndComboBoxes[i];
					minRect = rect.Top;
				}
			}
			log.AppendLine("  --File Name textbox found: hWnd={0}");
			log.AppendLine("    setting text to \"{0}\"", saveFilePath);
			WinApi.User32.SendMessage(hWndFileName, WinApi.User32.WM_SETTEXT, 0, saveFilePath);

			//click Save As button
			winTextSb = new StringBuilder(256);
			hWndChild = WinApi.User32.GetWindow(hWndSaveDialog, WinApi.User32.GW_CHILD);
			do
			{
				WinApi.User32.GetWindowText(hWndChild, winTextSb, winTextSb.Capacity);
				winText = winTextSb.ToString();
				if (SaveBtnText == winText)
				{
					log.AppendLine("  --Save button found: hWnd={0}, title=\"{1}\"", hWndChild, winText);
					log.AppendLine("    clicking button");
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONDOWN, 0, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONUP, 0, 0);
					WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
					System.Windows.Forms.MessageBox.Show("Releasing TaskProcessing");
					taskProcessingWaitHandle.Set(); //allow TaskProcessingLogic to continue with other tasks
				}
				hWndChild = WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT);
			} while (IntPtr.Zero != hWndChild);
		}

		private static Boolean EnumWindowsProcSaveAction(IntPtr hWnd, Int32 lParam)
		{
			StringBuilder titleSb = new StringBuilder(256);
			WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
			String title = titleSb.ToString();
			log.AppendLine("Window: hWnd={0}, title=\"{1}\"", hWnd, title);

			if (SaveDialogCaption == title)
			{
				Thread currThrd = Thread.CurrentThread;
				System.Windows.Forms.MessageBox.Show(String.Format(
					"In EnumWindows callback\nthread: {0}\nId: {1}\napplication context: {2}",
					currThrd.Name,
					currThrd.ManagedThreadId.ToString(),
					AppDomain.CurrentDomain.FriendlyName));
				log.AppendLine("--target window found");
				hWndSaveDialog = hWnd;
				return false;
			}

			return true;
		}

		//todo: prevent Bluebeam from opening
	}
}
