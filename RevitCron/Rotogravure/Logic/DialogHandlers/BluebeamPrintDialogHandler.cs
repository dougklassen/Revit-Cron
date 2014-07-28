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
            IntPtr hWnd = WinApi.User32.FindWindow("", SaveDialogCaption);
            log.AppendLine("Window found: Title=\"{0}\" hWnd={1}", SaveDialogCaption, hWnd);
            WinApi.User32.EnumWindows(EnumWindowsProcSaveAction, 0);
        }

        private static Boolean EnumWindowsProcSaveAction( IntPtr hWnd, Int32 lParam)
        {
            StringBuilder titleSb = new StringBuilder(256);
            WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
            String title = titleSb.ToString();
            log.AppendLine("Window: hWnd={0}, title=\"{1}\"", hWnd, title);

            try
            {
                System.IntPtr hWndChild = WinApi.User32.GetWindow(hWnd, WinApi.User32.GW_CHILD);
                if (IntPtr.Zero != hWndChild)
                {
                    StringBuilder buttonSb = new StringBuilder();
                    String buttonTitle;
                    do
                    {
                        WinApi.User32.GetWindowText(hWndChild, buttonSb, buttonSb.Capacity);
                        buttonTitle = buttonSb.ToString();
                        log.AppendLine("  button: hWnd={0}, title=\"{1}\"", hWndChild, buttonTitle);
                    } while (IntPtr.Zero != WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT));
                }
                else
                {
                    log.AppendLine("  --no child windows found");
                }
            }
            catch (Exception exc)
            {
                log.LogException(exc);
            }
            return true;
            //if (SaveDialogCaption == title)
            //{
            //    log.AppendLine("---target window found");
            //    System.IntPtr hWndChild = WinApi.User32.GetWindow(hWnd, WinApi.User32.GW_CHILD);
            //    StringBuilder buttonSb = new StringBuilder();
            //    String buttonTitle;
            //    do
            //    {
            //        WinApi.User32.GetWindowText(hWndChild, buttonSb, buttonSb.Capacity);
            //        buttonTitle = buttonSb.ToString();
            //        log.AppendLine("  button: hWnd={0}, title=\"{1}\"", hWndChild, buttonTitle);
            //    } while (IntPtr.Zero != WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT));


            //    //WinApi.User32.EnumChildWindows(hWnd, EnumChildProcSaveButton, 0);
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        private static Boolean EnumChildProcSaveButton( IntPtr hWnd, Int32 lParam)
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
