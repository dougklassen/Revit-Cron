using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron
{
	public static class RevitInstall
	{
		/// <summary>
		/// The information needed by Process.Start() to launch Revit
		/// </summary>
		public static ProcessStartInfo StartInfo = new ProcessStartInfo()
			{
				FileName = @"C:\Program Files\Autodesk\Revit 2015\Revit.exe",
				WorkingDirectory = @"C:\Program Files\Autodesk\Revit 2015\",
				Arguments = @"/language ENU"
			};
	}

	public static class RCronFileLocations
	{
		public static readonly String AddInDirectoryPath = @"C:\ProgramData\Autodesk\Revit\Addins\2015\Rotogravure\";
		public static readonly String ResourcesDirectoryPath = AddInDirectoryPath + @"Resources\";
		public static readonly String OptionsFilePath = AddInDirectoryPath + @"Resources\options.json";
		public static readonly String BatchFilePath = AddInDirectoryPath + @"Resources\batch.json";
		public static readonly String ScheduleFilePath = AddInDirectoryPath + @"Resources\schedule.json";
		public static readonly String LogDirectoryPath = AddInDirectoryPath + @"Logs\";
	}

	public static class RCronUris
	{
		public static readonly Uri AddInDirectoryUri = new Uri(RCronFileLocations.AddInDirectoryPath);
		public static readonly Uri ResourcesDirectoryUri = new Uri(RCronFileLocations.ResourcesDirectoryPath);
		public static readonly Uri OptionsFileUri = new Uri(RCronFileLocations.OptionsFilePath);
		public static readonly Uri BatchFileUri = new Uri(RCronFileLocations.BatchFilePath);
		public static readonly Uri ScheduleFileUri = new Uri(RCronFileLocations.ScheduleFilePath);
		public static readonly Uri LogDirectoryUri = new Uri(RCronFileLocations.LogDirectoryPath);		
	}

	public static class RCronCanon
	{
		/// <summary>
		/// Returns a timestamp for the current time in the format used by RCron
		/// </summary>
		public static String TimeStamp
		{
			get
			{
				return GetTimeStamp(DateTime.Now);
			}
		}

		/// <summary>
		/// Generates a timestamp from the specified time in the format used by RCron
		/// </summary>
		/// <param name="time">The specified time</param>
		/// <returns>An RCron format timestamp</returns>
		public static String GetTimeStamp(DateTime time)
		{
			StringBuilder timestamp = new StringBuilder(String.Empty);
			timestamp.Append(time.Year);
			timestamp.AppendFormat("{0:D2}{1:D2}_{2:D2}{3:D2}",
				time.Month,
				time.Day,
				time.Hour,
				time.Minute);
			return timestamp.ToString();
		}

		/// <summary>
		/// Generates a name for a new local copy of a central file
		/// </summary>
		/// <param name="centralFilePath">The path of the central file</param>
		/// <returns>A file path with local name based on a timestamp</returns>
		public static String GetLocalFileName(String centralFileName)
		{
			String localName = String.Empty;

			String localSuffix = "_loc" + TimeStamp;
			Regex.Replace(centralFileName, @"(?:=\.rvt$)", localSuffix, RegexOptions.IgnoreCase);

			return localName;
		}
	}

  //todo: use string keys for enum
	/// <summary>
	/// Recognized task types in Rotogravure
	/// </summary>
	public enum RCronTaskType
	{
		Print, Export, ETransmit, Command, Test, AuditCompact
	}
}
