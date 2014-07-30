using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Shapes;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public static class BluebeamPrintDialogHandler
    {
        private static RCronLog log = RCronLog.Instance;
        private static String saveFilePath;

        private const String SaveDialogCaption = "Save As";
        private const String SaveBtnText = "&Save";
        private const Double WaitTime = 3000; //todo: read from RotogravureOptions
        private static Timer waitTimer;

        public static void Save(String filePath)
        {
            saveFilePath = filePath;
            waitTimer = new Timer(WaitTime);
            waitTimer.Elapsed += WaitTimerElapsedSaveAction;
            log.AppendLine("Starting {0}MS timer", WaitTime);
            waitTimer.Start();
        }

        private static void WaitTimerElapsedSaveAction(object sender, ElapsedEventArgs e)
        {
            waitTimer.Stop();
            log.AppendLine("Timer elapsed");

            WinApi.User32.EnumWindows(EnumWindowsProcSaveAction, 0);
        }

        private static Boolean EnumWindowsProcSaveAction( IntPtr hWnd, Int32 lParam)
        {
            StringBuilder titleSb = new StringBuilder(256);
            WinApi.User32.GetWindowText(hWnd, titleSb, titleSb.Capacity);
            String title = titleSb.ToString();
            log.AppendLine("Window: hWnd={0}, title=\"{1}\"", hWnd, title);

            if (SaveDialogCaption == title)
            {
                log.AppendLine("--target window found");
                System.IntPtr hWndChild;
                StringBuilder winTextSb = new StringBuilder(256);
                String winText;

                //set save location
                List<IntPtr> hWndComboBoxes = new List<IntPtr>();
                hWndChild = WinApi.User32.GetWindow(hWnd, WinApi.User32.GW_CHILD);
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
                hWndChild = WinApi.User32.GetWindow(hWnd, WinApi.User32.GW_CHILD);
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
                        return false;    //since the button was found, stop enumeration of child windows
                    }
                    hWndChild = WinApi.User32.GetWindow(hWndChild, WinApi.User32.GW_HWNDNEXT);
                } while (IntPtr.Zero != hWndChild);
            }

            return true;    //continue enumeration of child windows
        }

        //todo: prevent Bluebeam from opening
    }
}
