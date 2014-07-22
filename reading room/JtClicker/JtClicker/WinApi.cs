using System;
using System.Runtime.InteropServices;
using System.Text;

namespace JtClicker
{
	public class WinApi
	{
		public class User32
		{
			private User32()
			{
			}

			public delegate bool EnumWindowsProc( 
        int hWnd, 
        int lParam );

      [DllImport("user32.Dll")]
      public static extern int FindWindow( 
        string className, 
        string windowName );

      [DllImport("user32.Dll")]
			public static extern int EnumWindows( 
        EnumWindowsProc callbackFunc, 
        int lParam );

			[DllImport("user32.Dll")]
			public static extern int EnumChildWindows( 
        int hwnd, 
        EnumWindowsProc callbackFunc, 
        int lParam );

			[DllImport("user32.Dll")]
			public static extern int GetWindowText( 
        int hwnd, 
        StringBuilder buff, 
        int maxCount );

			[DllImport("user32.Dll")]
			public static extern int GetLastActivePopup( 
        int hwnd );

      [DllImport( "user32.Dll" )]
      public static extern int SendMessage( 
        int hwnd, 
        int Msg, 
        int wParam, 
        int lParam );

      public const int BM_SETSTATE = 0x00F3;
      public const int WM_LBUTTONDOWN = 0x0201;
      public const int WM_LBUTTONUP = 0x0202;

		}
	}
}
