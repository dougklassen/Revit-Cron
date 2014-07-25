using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron
{
    public static class RCronFileLocations
    {
        public static readonly String AddInDirectoryPath = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\";
        public static readonly String OptionsFilePath = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Resources\ini.json";
    }

    public static class RCronCanon
    {
        public static String TimeStamp
        {
            get
            {
                StringBuilder timestamp = new StringBuilder(String.Empty);
                DateTime now = DateTime.Now;
                timestamp.Append(now.Year);
                timestamp.AppendFormat("{0:D2}", now.Month);
                timestamp.AppendFormat("{0:D2}", now.Day);
                timestamp.Append('_');
                timestamp.Append(now.ToFileTimeUtc().ToString());
                return timestamp.ToString();
            }
        }

        static RCronCanon() { }
    }
}
