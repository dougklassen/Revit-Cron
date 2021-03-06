﻿using System;
using System.Windows.Automation;

namespace DougKlassen.Revit.Automation
{
	class Program
	{
		static void Main(string[] args)
		{
			var revit = RevitHandler.GetRevitWindow();
			var adButton = RevitHandler.GetAdAppButton();

			if (revit != null)
			{
				Console.WriteLine("Revit Found");
				Console.WriteLine("** Name: {0}", revit.GetCurrentPropertyValue(AutomationElement.NameProperty));
				Console.WriteLine("** Native Window Property: {0}", revit.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty));

				if (adButton != null)
				{
					Console.WriteLine("AdApplication button found");

					var patterns = adButton.GetSupportedPatterns();
					Console.WriteLine(" Patterns:");
					foreach (var patt in patterns)
					{
						Console.WriteLine( " - {0}", patt.ProgrammaticName);
					}
					var props = adButton.GetSupportedProperties();
					Console.WriteLine(" Properties:");
					foreach (var prop in props)
					{
						Console.WriteLine(" - {0}", prop.ProgrammaticName);
						Console.WriteLine("   + {0}", adButton.GetCurrentPropertyValue(prop));
					}
				}

				PropertyCondition contentPropCondition = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
				var items = revit.FindAll(TreeScope.Children, contentPropCondition);

				Console.WriteLine("\nControls found:");
				foreach (AutomationElement child in items)
				{
					Console.WriteLine("Name: {0}", child.GetCurrentPropertyValue(AutomationElement.NameProperty));
					Console.WriteLine("Class: {0}", child.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
					ControlType ct = child.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty) as ControlType;
					Console.WriteLine("Control Type: {0}", ct.ProgrammaticName);

					Console.WriteLine("* * *");
				}
			}
			else
			{
				Console.WriteLine("Revit not found");
			}			

			Console.Write("\nPress any key to continue");
			Console.ReadKey();
		}
	}
}
