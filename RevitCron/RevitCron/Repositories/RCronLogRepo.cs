using DougKlassen.Revit.Cron.Models;

using System;
using System.IO;

namespace DougKlassen.Revit.Cron.Repositories
{
	/// <summary>
	/// A repository for the RevitCron log
	/// </summary>
	public interface IRCronLogRepo
	{
		/// <summary>
		/// Get a copy of the log from the repository
		/// </summary>
		/// <returns>An RCronLog object that can record RevitCron information</returns>
		RCronLog GetLog();

		/// <summary>
		/// Write log information to the repository
		/// </summary>
		/// <param name="log">An object with log information to be written to the repository</param>
		void PutLog(RCronLog log);
	}

	/// <summary>
	/// A repository for the RevitCron log stored in a file
	/// </summary>
	public class RCronLogFileRepo : IRCronLogRepo
	{
		private Uri logFileUri;

		private RCronLogFileRepo() { }

		/// <summary>
		/// Create a new instance of RCronLogFileRepo
		/// </summary>
		/// <param name="uri">A Uri representing the file location</param>
		public RCronLogFileRepo(Uri uri)
		{
			if (!uri.IsFile)
			{
				throw new ArgumentException(uri.ToString() + " is not a file URI");
			}
			logFileUri = uri;
		}

		/// <summary>
		/// Get a representation of the log stored in the repository file
		/// </summary>
		/// <returns>An object representing the log information stored in the repository file</returns>
		public RCronLog GetLog()
		{
			RCronLog log = RCronLog.Instance;
			log.Text = File.ReadAllText(logFileUri.LocalPath); //overwrite log contents with file contents
			//todo: when to clean up log?
			return log;
		}

		/// <summary>
		/// Write the log information to the repository file
		/// </summary>
		/// <param name="log"></param>
		public void PutLog(RCronLog log)
		{
			File.WriteAllText(logFileUri.LocalPath, log.Text);
		}

		/// <summary>
		/// Retrieve log information from a file location
		/// </summary>
		/// <param name="uri">A URI representing the file location</param>
		/// <returns>An object representing the log information stored in the repository file</returns>
		public static RCronLog LoadLog(Uri uri) //convenience method to load log from file
		{
			RCronLogFileRepo repo = new RCronLogFileRepo(uri);
			return repo.GetLog();
		}

		/// <summary>
		/// Write log information to a file location
		/// </summary>
		/// <param name="uri">A URI representing the file location</param>
		/// <param name="log">An object representing the log information to be written to the repository file</param>
		/// <remarks>If a log file already exists at that location, it will be overwritten</remarks>
		public static void WriteLog(Uri uri, RCronLog log)
		{
			RCronLogFileRepo repo = new RCronLogFileRepo(uri);
			repo.PutLog(log);
		}
	}
}
