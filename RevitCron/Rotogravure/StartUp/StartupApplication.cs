using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;

using DougKlassen.Revit.Cron.Rotogravure.Interface;
using DougKlassen.Revit.Cron.Rotogravure.Logic;

namespace DougKlassen.Revit.Cron.Rotogravure.StartUp
{
    public class StartUpApp : IExternalApplication
    {
        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.ApplicationInitialized += TaskProcessingLogic.OnApplicationInitialized;

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            RotogravureOptions options;
            RCronLog log = RCronLog.Instance;
            try
            {
                options = RotogravureOptionsJsonRepo.LoadOptions(new Uri(RCronFileLocations.OptionsFilePath));
            }
            catch (Exception exc)
            {
                String defaultLogDirectoryPath = RCronFileLocations.AddInDirectoryPath + @"\Logs\"; //hardcoded only if RotogravureOptions can't be loaded
                if (!Directory.Exists(defaultLogDirectoryPath))
                {
                    Directory.CreateDirectory(defaultLogDirectoryPath);
                }
                options = new RotogravureOptions()
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
