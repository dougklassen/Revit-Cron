using System;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	public class RCronOptions
	{
		[DataMember(Order = 0)]
		public Uri TasksFileUri { get; set; }

		[DataMember(Order = 1)]
		public Uri LogDirectoryUri { get; set; }
	}
}