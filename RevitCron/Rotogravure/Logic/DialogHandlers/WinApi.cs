using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public class WinApi
    {
        public class User32
        {
            public const int BM_SETSTATE = 0x00F3;
            public const int WM_LBUTTONDOWN = 0x0201;
            public const int WM_LBUTTONUP = 0x0202;

            private User32() {}

            public delegate Boolean EnumWindowsProc(Int32 hWnd, int lParam);

            [DllImport("user32.dll")]
            public static extern Int32 FindWindow(String className, String windowName);

            [DllImport("user32.dll")]
            public static extern Int32 EnumWindows(EnumWindowsProc callbackFunc, Int32 lparam);

            [DllImport("users32.dll")]
            public static extern Int32 EnumChildWindows(Int32 hWnd, EnumWindowsProc callbackFunc, Int32 lParam);

            [DllImport("user32.dll")]
            public static extern Int32 GetWindowText(Int32 hWnd, StringBuilder buff, Int32 maxCount);

            [DllImport("user32.dll")]
            public static extern Int32 GetLastActivePopup(Int32 hWnd);

            [DllImport("user32.dll")]
            public static extern Int32 SendMessage(Int32 hWnd, Int32 msg, Int32 wParam, Int32 lParam);
        }
    }
}
