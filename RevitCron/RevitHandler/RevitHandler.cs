using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DougKlassen.RevitHandler
{
	public static class RevitHandler
	{
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
					if (name.StartsWith("Autodesk Revit"))
					{
						revit = window;
					}
				}

				return revit;
			}
		}
	}
}
