﻿using DougKlassen.Revit.Cron.Models;

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

			using (FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Open))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronOptions));
				options = (RCronOptions)s.ReadObject(fs);
			}

			return options;
		}

		public void PutRCronOptions(RCronOptions options)
		{
			using (FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Create))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronOptions));
				s.WriteObject(fs, options);
			}
		}

		public static RCronOptions LoadOptions(Uri uri) //convenience method to load options from file
		{
			RCronOptionsJsonRepo repo = new RCronOptionsJsonRepo(uri);
			return repo.GetRCronOptions();
		}

		public static void WriteOptions(Uri uri, RCronOptions options) //convenience method to write options to file
		{
			RCronOptionsJsonRepo repo = new RCronOptionsJsonRepo(uri);
			repo.PutRCronOptions(options);
		}
	}
}
