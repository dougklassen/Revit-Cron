using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DougKlassen.RevitHandler
{
	class Program
	{
		static void Main(string[] args)
		{
			var revit = RevitHandler.RevitWindow;

			if (revit != null)
			{
				Console.WriteLine("Revit Found");
				Console.WriteLine("Name: {0}", revit.GetCurrentPropertyValue(AutomationElement.NameProperty));
				Console.WriteLine("Native Window Property: {0}", revit.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty));
				Console.WriteLine("Class Name: {0}", revit.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
			}
			else
			{
				Console.WriteLine("Revit not found");
			}

			PropertyCondition contentPropCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
			var windows = AutomationElement.RootElement.FindAll(TreeScope.Children, contentPropCondition);

			Console.WriteLine("\nWindows found:");
			foreach (AutomationElement window in windows)
			{
				if (true )
				{
					Console.WriteLine("Window: {0}", window.GetCurrentPropertyValue(AutomationElement.NameProperty));
				}
			}

			Console.Write("\nPress any key to continue");
			Console.ReadKey();
		}
	}
}
