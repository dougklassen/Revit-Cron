﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DougKlassen.RevitHandler
{
	/// <summary>
	/// A class supporting Revit UI automation
	/// </summary>
	public static class RevitHandler
	{
		/// <summary>
		/// A reference to the Revit main window
		/// </summary>
		public static AutomationElement RevitWindow
		{
			get
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
		}

		/// <summary>
		/// A reference to the "AdApplicationButton" which holds the Revit program menu
		/// </summary>
		public static AutomationElement AdAppButton
		{
			get
			{
				AutomationElement button;
				PropertyCondition cond = new PropertyCondition(AutomationElement.NameProperty, "AdApplicationButton");
				button = RevitWindow.FindFirst(TreeScope.Children, cond);
				return button;
			}
		}
	}
}