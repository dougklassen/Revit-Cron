using DougKlassen.Revit.Cron.Models;

using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DougKlassen.Revit.Cron.Repositories
{
	public interface IRotogravureOptionsRepo
	{
		RCronOptions GetRotogravureOptions();

		void PutRotogravureOptions(RCronOptions options);
	}

	public class RotogravureOptionsJsonRepo : IRotogravureOptionsRepo
	{
		private Uri repoFileUri;

		private RotogravureOptionsJsonRepo() { }

		public RotogravureOptionsJsonRepo(Uri uri)
			: this()
		{
			repoFileUri = uri;
		}

		public RCronOptions GetRotogravureOptions()
		{
			RCronOptions options = null;

			using (FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Open))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronOptions));
				options = (RCronOptions)s.ReadObject(fs);
			}

			return options;
		}

		public void PutRotogravureOptions(RCronOptions options)
		{
			using (FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Create))
			{
				DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RCronOptions));
				s.WriteObject(fs, options);
			}
		}

		public static RCronOptions LoadOptions(Uri uri) //convenience method to load options from file
		{
			RotogravureOptionsJsonRepo repo = new RotogravureOptionsJsonRepo(uri);
			return repo.GetRotogravureOptions();
		}

		public static void WriteOptions(Uri uri, RCronOptions options) //convenience method to write options to file
		{
			RotogravureOptionsJsonRepo repo = new RotogravureOptionsJsonRepo(uri);
			repo.PutRotogravureOptions(options);
		}
	}
}
