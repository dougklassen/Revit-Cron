﻿using System;
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
                if (!Directory.Exists(RCronFileLocations.LogDirectoryPath))
                {
                    Directory.CreateDirectory(RCronFileLocations.LogDirectoryPath);
                }
                String path = RCronFileLocations.LogDirectoryPath + RCronCanon.TimeStamp + @"_error.txt";
                options = new RotogravureOptions()
                    {
                        LogFileUri = new Uri(path)
                    };
                log.LogException(exc);
            }
            try
            {
                RCronLogFileRepo.WriteLog(options.LogFileUri, log);   //write the log file to disk
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
