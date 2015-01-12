using DougKlassen.Revit.Cron.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRCronScheduleRepo
	{
		RCronSchedule GetRCronSchedule();

		void PutRCronSchedule(RCronSchedule schedule);
	}

	public class RCronScheduleJsonRepo : IRCronScheduleRepo
	{
		private String repoFilePath;

		private RCronScheduleJsonRepo() { }	//don't want a repo initialized without a file to point to

		public RCronScheduleJsonRepo(Uri fileUri)
			: this()
		{
			if (fileUri.IsFile)
			{
				repoFilePath = fileUri.LocalPath;
			}
			else
			{
				throw new ArgumentException("Schedule URI was not a file URI");
			}
		}

		public RCronSchedule GetRCronSchedule()
		{
			RCronSchedule schedule = null;
			var js = GetJsonSerializer();

			using (var sr = new StreamReader(repoFilePath))
			using (var reader = new JsonTextReader(sr))
			{
				schedule = js.Deserialize<RCronSchedule>(reader);
			}

			return schedule;
		}

		public void PutRCronSchedule(RCronSchedule schedule)
		{
			var js = GetJsonSerializer();

			using (var sw = new StreamWriter(repoFilePath))
			using (var writer = new JsonTextWriter(sw))
			{
				js.Serialize(writer, schedule);
			}
		}

		/// <summary>
		/// Convenience method to load a schedule from a URI
		/// </summary>
		/// <param name="uri">The location of the JSON repository</param>
		/// <returns>The schedule loaded from the file</returns>
		public static RCronSchedule LoadSchedule(Uri uri)
		{
			RCronScheduleJsonRepo repo = new RCronScheduleJsonRepo(uri);
			return repo.GetRCronSchedule();
		}

		/// <summary>
		/// Convenience method to write a schedule to a URI
		/// </summary>
		/// <param name="uri">The destination for the JSON repository</param>
		/// <param name="schedule">The schedule to write</param>
		public static void WriteSchedule(Uri uri, RCronSchedule schedule)
		{
			RCronScheduleJsonRepo repo = new RCronScheduleJsonRepo(uri);
			repo.PutRCronSchedule(schedule);
		}

		/// <summary>
		/// Get the JsonSerializer to use with RCronSchedule
		/// </summary>
		/// <returns></returns>
		private JsonSerializer GetJsonSerializer()
		{
			var serializer = new JsonSerializer();
			serializer.Formatting = Formatting.Indented;
			serializer.Converters.Add(new RCronTaskSpecConverter());
			return serializer;
		}
	}
}