using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            application.DialogBoxShowing += DialogEventHandler.OnDialogShowing;

            return Result.Succeeded;
        }

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }        
    }
}
