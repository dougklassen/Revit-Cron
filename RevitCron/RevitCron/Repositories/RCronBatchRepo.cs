using DougKlassen.Revit.Cron.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRCronBatchRepo
	{
		RCronBatch GetRCronBatch();

		void PutRCronBatch(RCronBatch batch);
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
				throw new ArgumentException("Batch file URI was not a file URI");
			}
		}

		public RCronBatch GetRCronBatch()
		{
			RCronBatch batch = null;

			using (FileStream fs = new FileStream(repoFilePath, FileMode.Open))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronBatch));
				batch = (RCronBatch)s.ReadObject(fs);
			}

			return batch;
		}

		public void PutRCronBatch(RCronBatch batch)
		{
			using (FileStream fs = new FileStream(repoFilePath, FileMode.Create))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronBatch));
				s.WriteObject(fs, batch);
			}
		}

		public static RCronBatch LoadBatch(Uri uri) //convenience method to load tasks from a file
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(uri);
			return repo.GetRCronBatch();
		}

		public static void WriteBatch(Uri uri, RCronBatch batch)
		{
			RCronBatchJsonRepo repo = new RCronBatchJsonRepo(uri);
			repo.PutRCronBatch(batch);
		}
	}
}