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
        public static String RotogravureAssemblyName = "Rotogravure";
        public static String OptionsFilePath = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\Resources\ini.json";
        public static readonly String ImperialTemplateDirectoryPath = @"C:\ProgramData\Autodesk\RVT 2014\Family Templates\English_I\";
        public static readonly String ResourceNameSpace = @"Rotogravure.Resources";

        public static Uri GetUri(String filePath)
        {
            String uriString = "file:///" + Regex.Replace(filePath, @"\\", @"/");
            return new Uri(uriString);
        }
    }
}
