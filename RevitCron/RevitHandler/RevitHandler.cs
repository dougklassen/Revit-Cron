using Autodesk.Revit.UI.Events;
using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Threading;

namespace DougKlassen.Revit.Automation
{
	/// <summary>
	/// A class supporting Revit UI automation
	/// </summary>
	public sealed class RevitHandler
	{
		/// <summary>
		/// Field for specifying response behavior to 
		/// </summary>
		private DialogResponse DialogHandler { get; set; }

		/// <summary>
		/// Singleton static initializer
		/// </summary>
		private static readonly RevitHandler instance = new RevitHandler();

		/// <summary>
		/// Accessor for singleton instance
		/// </summary>
		public static RevitHandler Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Private default constructor
		/// </summary>
		private RevitHandler()
		{
			DialogHandler = new DialogResponse();
		}

		/// <summary>
		/// Get a reference to the Revit main window
		/// </summary>
		public static AutomationElement GetRevitWindow()
		{
			AutomationElement revit = null;

			PropertyCondition contentPropCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
			var windows = AutomationElement.RootElement.FindAll(TreeScope.Children, contentPropCondition);

			foreach (AutomationElement window in windows)
			{
				String name = window.GetCurrentPropertyValue(AutomationElement.NameProperty).ToString();
				if (name.StartsWith("Autodesk Revit "))
				{
					revit = window;
				}
			}

			return revit;
		}

		/// <summary>
		/// Get a reference to the "AdApplicationButton" which holds the Revit program menu
		/// </summary>
		public static AutomationElement GetAdAppButton()
		{
			var revit = GetRevitWindow();
			if (revit == null)
			{
				return null;
			}
			else
			{
				AutomationElement button;
				PropertyCondition cond = new PropertyCondition(AutomationElement.NameProperty, "AdApplicationButton");
				button = revit.FindFirst(TreeScope.Children, cond);
				return button;
			}
		}

		/// <summary>
		/// Close all active views, and consequently all project
		/// </summary>
		/// <param name="synchronize">Whether to synchronize on close</param>
		/// <param name="save">Whether to save on close</param>
		public void CloseActive(Boolean synchronize = false, Boolean save = false)
		{
			if (save)
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 6); //yes
			}
			else
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 7); //no
			}
			//var hWndRevit = GetRevitHandle();
			//var hWndRevit = WinApi.FindWindow(null, "AdImpApplicationFrame");
			var hWndRevit = WinApi.FindWindow(null, "Workspace");
			MessageBox.Show("Setting foreground window to " + hWndRevit);
			WinApi.SetForegroundWindow(hWndRevit);

			Thread.Sleep(5000);

			for (int i = 0; i < 10; i++)
			{
				Thread.Sleep(1000);
				SendCtlF4();
			}

			DialogHandler.ClearOverrides();
		}

		/// <summary>
		/// Exit Revit 
		/// </summary>
		public void Exit(Boolean synchronize = false, Boolean save = false)
		{
			if (save)
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 6); //yes
			}
			else
			{
				DialogHandler.AddOverride(RevitDialog.SaveFile, 7); //no
			}
			IntPtr hWndRevit = GetRevitHandle();
			WinApi.SendMessage(hWndRevit, WinApi.WM_CLOSE, 0, 0);
			DialogHandler.ClearOverrides();
		}

		/// <summary>
		/// Get a handle to Revit using Win32
		/// </summary>
		/// <returns>A handle to the Revit window</returns>
		/// <remarks>Assumes that Revit is the currently running program</remarks>
		private static IntPtr GetRevitHandle()
		{
			IntPtr hWndRevit = IntPtr.Zero;

			//Process process = Process.GetCurrentProcess();
			//hWndRevit = process.MainWindowHandle;

			var handle = (Int32)(GetRevitWindow().GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty));
			hWndRevit = (IntPtr)handle;

			return hWndRevit;
		}

		/// <summary>
		/// Send a button click to the specified button element
		/// </summary>
		/// <param name="hWndChild"></param>
		private static void ClickButton(IntPtr hWndChild)
		{
			WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
			WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONDOWN, 0, 0);
			WinApi.SendMessage(hWndChild, WinApi.WM_LBUTTONUP, 0, 0);
			WinApi.SendMessage(hWndChild, WinApi.BM_SETSTATE, 1, 0);
		}

		/// <summary>
		/// Send a Ctl-F4 key press to the current window
		/// </summary>
		private static void SendCtlF4()
		{
			var pInputs = new[]
			{
				new INPUT()
				{
					type = (UInt32)INPUT_TYPE.INPUT_KEYBOARD,
					U = new InputUnion()
					{
						ki = new KEYBDINPUT()
						{
							wScan = ScanCodeShort.CONTROL,
							wVk = VirtualKeyShort.CONTROL
						}
					}
				},
				new INPUT()
				{
					type = (UInt32)INPUT_TYPE.INPUT_KEYBOARD,
					U = new InputUnion()
					{
						ki = new KEYBDINPUT()
						{
							wScan = ScanCodeShort.F4,
							wVk = VirtualKeyShort.F4
						}
					}
				},
				new INPUT()
				{
					type = (UInt32)INPUT_TYPE.INPUT_KEYBOARD,
					U = new InputUnion()
					{
						ki = new KEYBDINPUT()
						{
							wScan = ScanCodeShort.CONTROL,
							wVk = VirtualKeyShort.CONTROL,
							dwFlags = KEYEVENTF.KEYUP
						}
					}
				},
				new INPUT()
				{
					type = (UInt32)INPUT_TYPE.INPUT_KEYBOARD,
					U = new InputUnion()
					{
						ki = new KEYBDINPUT()
						{
							wScan = ScanCodeShort.F4,
							wVk = VirtualKeyShort.F4,
							dwFlags = KEYEVENTF.KEYUP
						}
					}
				}
			};

			WinApi.SendInput(4, pInputs, INPUT.Size);
		}

		/// <summary>
		/// Carry out dialog handling according to current dialog handling settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnDialogShowing(Object sender, DialogBoxShowingEventArgs e)
		{
			var args = e as TaskDialogShowingEventArgs;
			if (args != null)
			{
				if (DialogHandler.HasOverride(args.DialogId))
				{
					args.OverrideResult(DialogHandler.GetOverride(args.DialogId));
				}
			}
		}
	}
}