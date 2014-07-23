using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DougKlassen.Revit.Cron.Rotogravure.Logic
{
    public class DialogEventHandler
    {
        private const int IDOK = 1;
        private const int IDCANCEL = 2;
        private const int IDABORT = 3;
        private const int IDRETRY = 4;
        private const int IDIGNORE = 5;
        private const int IDYES = 6;
        private const int IDNO = 7;

        private static StringBuilder logText = RCronLog.Instance.LogText;

        public static void OnDialogShowing(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
        {
            logText.AppendLine("Dialog showing");
            //e.OverrideResult(IDOK);
        }
    }
}
