using System;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	public class RCronOptions
	{
		/// <summary>
		/// The Batch file to check upon Revit startup
		/// todo: may want the option to process multiple batch files at once
		/// </summary>
		[DataMember(Order = 0)]
		public Uri BatchFileUri { get; set; }

		/// <summary>
		/// The Schedule file location
		/// </summary>
		[DataMember(Order = 1)]
		public Uri ScheduleFileUri { get; set; }

		/// <summary>
		/// The directory to write logs to
		/// </summary>
		[DataMember(Order = 2)]
		public Uri LogDirectoryUri { get; set; }

		/// <summary>
		/// Polling period for RCronD
		/// </summary>
		[DataMember(Order = 3)]
		public TimeSpan PollingPeriod { get; set; }

		/// <summary>
		/// The span of time in which to group sequential tasks into a single batch
		/// </summary>
		[DataMember(Order = 4)]
		public TimeSpan BatchSpan { get; set; }
	}
}