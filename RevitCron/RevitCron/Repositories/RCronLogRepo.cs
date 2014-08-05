using DougKlassen.Revit.Cron.Models;

using System;
using System.IO;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRCronLogRepo
	{
		RCronLog GetLog();

		void PutLog(RCronLog log);
	}

	public class RCronLogFileRepo : IRCronLogRepo
	{
		private Uri logFileUri;

		private RCronLogFileRepo() { }

		public RCronLogFileRepo(Uri uri)
		{
			if (!uri.IsFile)
			{
				throw new ArgumentException(uri.ToString() + " is not a file URI");
			}
			logFileUri = uri;
		}

		public RCronLog GetLog()
		{
			RCronLog log = RCronLog.Instance;
			log.Text = File.ReadAllText(logFileUri.LocalPath); //overwrite log contents with file contents
			//todo: when to clean up log?
			return log;
		}

		public void PutLog(RCronLog log)
		{
			File.WriteAllText(logFileUri.LocalPath, log.Text);
		}

		public static RCronLog LoadLog(Uri uri) //convenience method to load log from file
		{
			RCronLogFileRepo repo = new RCronLogFileRepo(uri);
			return repo.GetLog();
		}

		public static void WriteLog(Uri uri, RCronLog log)
		{
			RCronLogFileRepo repo = new RCronLogFileRepo(uri);
			repo.PutLog(log);
		}
	}
}
