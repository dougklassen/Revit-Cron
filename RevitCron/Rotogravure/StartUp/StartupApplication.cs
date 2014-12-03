using Autodesk.Revit.UI;
using DougKlassen.Revit.Automation;
using RevitHandler = DougKlassen.Revit.Automation.RevitHandler;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;
using DougKlassen.Revit.Cron.Rotogravure.Logic;
using System;
using System.IO;
using System.Windows.Forms;

namespace DougKlassen.Revit.Cron.Rotogravure.StartUp
{
	public class StartUpApp : IExternalApplication
	{
		Result IExternalApplication.OnStartup(UIControlledApplication application)
		{
			application.ControlledApplication.ApplicationInitialized += TaskProcessingLogic.OnApplicationInitialized;
			application.DialogBoxShowing += RevitHandler.OnDialogShowing;

			return Result.Succeeded;
		}

		Result IExternalApplication.OnShutdown(UIControlledApplication application)
		{
			RCronOptions options;
			RCronLog log = RCronLog.Instance;
			try
			{
				options = RCronOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
			}
			catch (Exception exc)
			{
				String defaultLogDirectoryPath = RCronFileLocations.AddInDirectoryPath + @"\Logs\"; //hardcoded only if RCronOptions can't be loaded
				if (!Directory.Exists(defaultLogDirectoryPath))
				{
					Directory.CreateDirectory(defaultLogDirectoryPath);
				}
				options = new RCronOptions()
						{
							LogDirectoryUri = new Uri(defaultLogDirectoryPath)
						};
				log.LogException(exc);
			}
			try
			{
				String logDirectoryPath = options.LogDirectoryUri.LocalPath;
				if (!Directory.Exists(logDirectoryPath))
				{
					Directory.CreateDirectory(logDirectoryPath);
				}
				Uri logFile = new Uri(options.LogDirectoryUri, RCronCanon.TimeStamp + "_log.txt");
				RCronLogFileRepo.WriteLog(logFile, log);   //write the log file to disk
			}
			catch (Exception exc)
			{
				MessageBox.Show("Could not write log\n\n" + exc.Message);
				return Result.Failed;
			}
			return Result.Succeeded;
		}
	}
}
