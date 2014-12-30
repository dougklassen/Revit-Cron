using DougKlassen.Revit.Cron.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRCronOptionsRepo
	{
		RCronOptions GetRCronOptions();

		void PutRCronOptions(RCronOptions options);
	}

	public class RCronOptionsJsonRepo : IRCronOptionsRepo
	{
		private Uri repoFileUri;

		private RCronOptionsJsonRepo() { }

		public RCronOptionsJsonRepo(Uri uri)
			: this()
		{
			repoFileUri = uri;
		}

		public RCronOptions GetRCronOptions()
		{
			RCronOptions options = null;
			var js = GetJsonSerializer();

			using (var sr = new StreamReader(repoFileUri.LocalPath))
			using (var reader = new JsonTextReader(sr))
			{
				options = js.Deserialize<RCronOptions>(reader);
			}

			return options;
		}

		public void PutRCronOptions(RCronOptions options)
		{
			var js = GetJsonSerializer();

			using (var sw = new StreamWriter(repoFileUri.LocalPath, false))
			using (var writer = new JsonTextWriter(sw))
			{
				js.Serialize(writer, options);
			}
		}

		/// <summary>
		/// Convenience method to load options from a file specified by a Uri
		/// </summary>
		/// <param name="uri">The Uri of the RCronOptions repo</param>
		/// <returns>Options for running RCron</returns>
		public static RCronOptions LoadOptions(Uri uri) //convenience method to load options from file
		{
			RCronOptionsJsonRepo repo = new RCronOptionsJsonRepo(uri);
			return repo.GetRCronOptions();
		}

		/// <summary>
		/// Convenience method to load options from a file specified by a file path
		/// </summary>
		/// <param name="filePath">The path of the RCronOptions repo</param>
		/// <returns>Options for running RCron</returns>
		public static RCronOptions LoadOptions(String filePath)
		{
			RCronOptionsJsonRepo repo = new RCronOptionsJsonRepo(new Uri(filePath));
			return repo.GetRCronOptions();
		}

		/// <summary>
		/// Convenience method to write options to a file specified by a Uri
		/// </summary>
		/// <param name="uri">The location of the repo to write the options to</param>
		/// <param name="options">The options to write</param>
		public static void WriteOptions(Uri uri, RCronOptions options) //convenience method to write options to file
		{
			RCronOptionsJsonRepo repo = new RCronOptionsJsonRepo(uri);
			repo.PutRCronOptions(options);
		}

		/// <summary>
		/// Get the JsonSerializer to use with RCronOptions
		/// </summary>
		/// <returns></returns>
		private JsonSerializer GetJsonSerializer()
		{
			var serializer = new JsonSerializer();
			serializer.Formatting = Formatting.Indented;
			return serializer;
		}
	}
}
