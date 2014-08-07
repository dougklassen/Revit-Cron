using System;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	public class RCronOptions
	{
		[DataMember(Order = 0)]
		public Uri BatchFileUri { get; set; }

		[DataMember(Order = 1)]
		public Uri ScheduleFileUri { get; set; }

		[DataMember(Order = 2)]
		public Uri LogDirectoryUri { get; set; }

		/// <summary>
		/// Polling period for RCronD
		/// </summary>
		[DataMember(Order = 3)]
		public TimeSpan PollingPeriod { get; set; }
	}
}