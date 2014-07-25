using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public static class BluebeamPrintDialogHandler
    {
        private static RCronLog log = RCronLog.Instance;

        private const String SaveDialogCaption = "Save As";
        private const String SaveBtnText = "Save";
        private const Double WaitTime = 5000; //todo: read from RotogravureOptions
        private static Timer waitTimer;

        public static void Save()
        {
            waitTimer = new Timer(WaitTime);
            waitTimer.Elapsed += WaitTimerElapsedSaveAction;
            log.AppendLine("Starting {0} timer", WaitTime);
            waitTimer.Start();
        }

        private static void WaitTimerElapsedSaveAction(object sender, ElapsedEventArgs e)
        {
            waitTimer.Stop();
            log.AppendLine("Timer elapsed");
            Int32 hWnd = WinApi.User32.FindWindow("", SaveDialogCaption);
            log.AppendLine("Window found: Title=\"{0}\" hWnd={1}", SaveDialogCaption, hWnd);
            WinApi.User32.EnumWindows(EnumWindowsProcSaveAction, 0);
        }

        private static Boolean EnumWindowsProcSaveAction( Int32 hWnd, Int32 lParam)
        {
            StringBuilder titleSb = new StringBuilder(256);
            WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
            String title = titleSb.ToString();
            log.AppendLine("Window: hWnd={0}, title=\"{1}\"", hWnd, title);

            if (SaveDialogCaption == title)
            {
                log.AppendLine("---target window found");
                WinApi.User32.EnumChildWindows(hWnd, EnumChildProcSaveButton, 0);
                return false;
            }
            else
	        {
                return true;
	        }
        }

        private static Boolean EnumChildProcSaveButton( Int32 hWnd, Int32 lParam)
        {
            StringBuilder titleSb = new StringBuilder(256);
            WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
            String title = titleSb.ToString();
            log.AppendLine("  button: hWnd={0}, title=\"{1}\"", hWnd, title);

            if (SaveBtnText == title)
            {
                log.AppendLine("  ---target button found");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
