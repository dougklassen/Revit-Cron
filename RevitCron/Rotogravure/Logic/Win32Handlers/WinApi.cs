using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
	public struct RECT
	{
		public int Left, Top, Right, Botton;
	}

	public class WinApi
	{
		public class User32
		{
			public const Int32 BM_SETSTATE = 0x00F3;
			public const Int32 WM_CLOSE = 0x0010;
			public const Int32 WM_LBUTTONDOWN = 0x0201;
			public const Int32 WM_LBUTTONUP = 0x0202;
			public const Int32 WM_SETTEXT = 0x000C;

			public const Int32 GW_CHILD = 5;
			public const Int32 GW_HWNDNEXT = 2;

			public delegate Boolean EnumWindowsProc(IntPtr hWnd, int lParam);

			[DllImport("user32.dll")]
			public static extern IntPtr FindWindow(String className, String windowName);

			[DllImport("user32.dll")]
			public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hWndChildAfter, String windowClass, String windowName);

			[DllImport("user32.dll")]
			public static extern Boolean EnumWindows(EnumWindowsProc callbackFunc, Int32 lparam);

			[DllImport("user32.dll")]
			public static extern Boolean EnumChildWindows(IntPtr hWnd, EnumWindowsProc callbackFunc, Int32 lParam);

			[DllImport("user32.dll")]
			public static extern Int32 GetClassName(IntPtr hWnd, StringBuilder sb, Int32 maxCount);

			[DllImport("user32.dll")]
			public static extern IntPtr GetWindow(IntPtr hWnd, Int32 uCmd);

			[DllImport("user32.dll")]
			public static extern Boolean GetWindowRect(IntPtr hWnd, ref RECT rect);

			[DllImport("user32.dll")]
			public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder sb, Int32 maxCount);

			[DllImport("user32.dll")]
			public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

			[DllImport("user32.dll")]
			public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, String lParam);

			[DllImport("user32.dll")]
			public static extern Int32 SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
		}
	}
}
