using System;
using System.Text;

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
				timestamp.AppendFormat("{0:D2}{1:D2}_{2:D2}{3:D2}",
					now.Month,
					now.Day,
					now.Hour,
					now.Minute);
				return timestamp.ToString();
			}
		}

		static RCronCanon() { }
	}
}
