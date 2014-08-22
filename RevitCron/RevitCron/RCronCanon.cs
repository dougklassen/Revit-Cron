using System;
using System.Text;

namespace DougKlassen.Revit.Cron
{
	public static class RCronFileLocations
	{
		public static readonly String AddInDirectoryPath = @"C:\ProgramData\Autodesk\Revit\Addins\2014\Rotogravure\";
		public static readonly String OptionsFilePath = AddInDirectoryPath + @"Resources\options.json";
		public static readonly String BatchFilePath = AddInDirectoryPath + @"Resources\batch.json";
		public static readonly String ScheduleFilePath = AddInDirectoryPath + @"Resources\schedule.json";
		public static readonly String LogDirectoryPath = AddInDirectoryPath + @"\Logs\";
	}

	public static class RCronUris
	{
		public static readonly Uri AddInDirectoryUri = new Uri(RCronFileLocations.AddInDirectoryPath);
		public static readonly Uri OptionsUri = new Uri(RCronFileLocations.OptionsFilePath);
		public static readonly Uri BatchUri = new Uri(RCronFileLocations.BatchFilePath);
		public static readonly Uri ScheduleUri = new Uri(RCronFileLocations.ScheduleFilePath);
		public static readonly Uri LogDirectoryUri = new Uri(RCronFileLocations.LogDirectoryPath);		
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

  //todo: use string keys for enum
	/// <summary>
	/// Recognized task types in Rotogravure
	/// </summary>
	public enum RCronTaskType
	{
		Print, Export, ETransmit, Command, Test
	}
}
