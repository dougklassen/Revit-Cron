using DougKlassen.Revit.Cron.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRCronBatchRepo
	{
		RCronBatch GetRCronBatch();

		void PutRCronBatch(RCronBatch batch);

		void Delete();
	}

	public class RCronBatchJsonRepo : IRCronBatchRepo
	{
		private String repoFilePath;

		private RCronBatchJsonRepo() { }

		public RCronBatchJsonRepo(Uri fileUri)
			: this()
		{
			if (fileUri.IsFile)
			{
				repoFilePath = fileUri.LocalPath;
			}
			else
			{
				throw new ArgumentException("Batch URI was not a file URI");
			}
		}

		public RCronBatch GetRCronBatch()
		{
			RCronBatch batch = null;

			var js = GetJsonSerializer();

			using (var sr = new StreamReader(repoFilePath))
			using (var reader = new JsonTextReader(sr))
			{
				batch = js.Deserialize<RCronBatch>(reader);
			}

			return batch;
		}

		public void PutRCronBatch(RCronBatch batch)
		{
			var js = GetJsonSerializer();

			using (var sw = new StreamWriter(repoFilePath, false))
			using (var writer = new JsonTextWriter(sw))
			{
				js.Serialize(writer, batch);
			}
		}

		/// <summary>
		/// Flush the repository by deleting the temporary batch file
		/// </summary>
		public void Delete()
		{
			File.Delete(repoFilePath);
			repoFilePath = null;
		}

		/// <summary>
		/// Convenience method to load a batch from a file specified by a URI
		/// </summary>
		/// <param name="uri">The Uri of the location to load from</param>
		/// <returns>The loaded batch</returns>
		public static RCronBatch LoadBatch(Uri uri) //convenience method to load tasks from a file specified by a URI
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(uri);
			return repo.GetRCronBatch();
		}

		/// <summary>
		/// Convenience method to load a batch from a file specified by a path
		/// </summary>
		/// <param name="filePath">The path of the location to load from</param>
		/// <returns>The loaded batch</returns>
		public static RCronBatch LoadBatch(String filePath)	//convenience method to load tasks from a file specified by a path
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(new Uri(filePath));
			return repo.GetRCronBatch();
		}

		/// <summary>
		/// Convenience method to write a batch to a repo specified by a Uri
		/// </summary>
		/// <param name="uri">The Uri to write to</param>
		/// <param name="batch">The batch to write</param>
		public static void WriteBatch(Uri uri, RCronBatch batch)
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(uri);
			repo.PutRCronBatch(batch);
		}

		/// <summary>
		/// Convenience method to write a batch to a repo specified by a file path
		/// </summary>
		/// <param name="filePath">The path of the location to write to</param>
		/// <param name="batch">The batch to write</param>
		public static void WriteBatch(String filePath, RCronBatch batch)
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(new Uri(filePath));
			repo.PutRCronBatch(batch);
		}

		/// <summary>
		/// Get the JsonSerializer to use with RCronOptions
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