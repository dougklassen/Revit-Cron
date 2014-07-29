using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public class BluebeamPrintDialogHandler
    {
        private static RCronLog log = RCronLog.Instance;
        private RCronTask task;

        private const String SaveDialogCaption = "Save As";
        private const String SaveBtnText = "&Save";
        private const Double WaitTime = 5000; //todo: read from RotogravureOptions
        private static Timer waitTimer;

        public BluebeamPrintDialogHandler(RCronTask t)
        {
            task = t;
        }

        public void Save()
        {
            waitTimer = new Timer(WaitTime);
            waitTimer.Elapsed += WaitTimerElapsedSaveAction;
            log.AppendLine("Starting {0} timer", WaitTime);
            waitTimer.Start();
        }

        private void WaitTimerElapsedSaveAction(object sender, ElapsedEventArgs e)
        {
            waitTimer.Stop();
            log.AppendLine("Timer elapsed");
            IntPtr hWnd = WinApi.User32.FindWindow("", SaveDialogCaption);
            log.AppendLine("Window found: Title=\"{0}\" hWnd={1}", SaveDialogCaption, hWnd);
            WinApi.User32.EnumWindows(EnumWindowsProcSaveAction, 0);
        }

        private Boolean EnumWindowsProcSaveAction( IntPtr hWnd, Int32 lParam)
        {
            StringBuilder titleSb = new StringBuilder(256);
            WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
            String title = titleSb.ToString();
            log.AppendLine("Window: hWnd={0}, title=\"{1}\"", hWnd, title);

            if (SaveDialogCaption == title)
            {
                log.AppendLine("---target window found");
                System.IntPtr hWndChild = WinApi.User32.GetWindow(hWnd, WinApi.User32.GW_CHILD);
                StringBuilder buttonSb = new StringBuilder(256);
                String buttonTitle;
                do
                {
                    ////set file name
                    //IntPtr hWndFileNameEdit = (IntPtr) 0x0030940;
                    //String outputPath = ((RCronPrintTaskInfo)task.TaskInfo).OutputFilePath;
                    //WinApi.User32.SendMessage(hWndFileNameEdit, WinApi.User32.WM_SETTEXT, 0, outputPath);
                    //hWndFileNameEdit = (IntPtr)0x004093A;
                    //WinApi.User32.SendMessage(hWndFileNameEdit, WinApi.User32.WM_SETTEXT, 0, outputPath);
                    //hWndFileNameEdit = (IntPtr)0x00209A4;
                    //WinApi.User32.SendMessage(hWndFileNameEdit, WinApi.User32.WM_SETTEXT, 0, outputPath);

                    //click button
                    WinApi.User32.GetWindowText(hWndChild, buttonSb, buttonSb.Capacity);
                    buttonTitle = buttonSb.ToString();
                    log.AppendLine("  button: hWnd={0}, title=\"{1}\"", hWndChild, buttonTitle);
                    if (SaveBtnText == buttonTitle)
                    {
                        log.AppendLine("  --clicking button");
                        WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
                        WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONDOWN, 0, 0);
                        WinApi.User32.SendMessage(hWndChild, WinApi.User32.WM_LBUTTONUP, 0, 0);
                        WinApi.User32.SendMessage(hWndChild, WinApi.User32.BM_SETSTATE, 1, 0);
                    }
                    hWndChild = WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT);
                } while (IntPtr.Zero != hWndChild);

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
