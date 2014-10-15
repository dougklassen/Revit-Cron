using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	[KnownType(typeof(RCronPrintTaskSpec))]
	[KnownType(typeof(RCronExportTaskSpec))]
	[KnownType(typeof(RCronETransmitTaskSpec))]
	[KnownType(typeof(RCronCommandTaskSpec))]
	[KnownType(typeof(RCronTestTaskSpec))]
	public class RCronTask
	{
		#region properties
		/// <summary>
		/// The name of the task, used as its unique ID
		/// </summary>
		[DataMember(Order = 0)]
		public String Name { get; set; }

		/// <summary>
		/// Controls the order of tasks when batched together. Higher priority tasks run first.
		/// Use is optional at this point.
		/// </summary>
		[DataMember(Order = 10)]
		public Int32 Priority { get; set; }

		/// <summary>
		/// Whether to run the task immediately if it wasn't run at the last scheduled run time.
		/// Not currently implemented
		/// </summary>
		[DataMember(Order = 11)]
		public Boolean RunIfMissed { get; set; }

		/// <summary>
		/// A statement of when the task should be run, in Cron format
		/// </summary>
		[DataMember(Order = 30)]
		public String Schedule { get; set; }

		/// <summary>
		/// The last time the task was run, or null if it hasn't been run
		/// </summary>
		[DataMember(Order = 20)]
		public DateTime? LastRun { get; set; }

		/// <summary>
		/// The information Revit needs to run the task, encapsulated in a RCronTaskSpec
		/// </summary>
		[DataMember(Order = 10)]
		public RCronTaskSpec TaskSpec { get; set; } //to facilitate serialization, subclassed members belong to a member class
		#endregion properties

		#region methods
		/// <summary>
		/// Gets the first scheduled run time after the specified DateTime
		/// </summary>
		/// <param name="afterTime">The time after which the task should be scheduled</param>
		/// <returns>The next run time</returns>
		public DateTime NextRunTime(DateTime afterTime)
		{
			CronExpression schedCron = new CronExpression(Schedule);
			DateTime nextRun = schedCron.GetAnnualRunTimes(afterTime.Year).Where(r => r > afterTime).Min();
			return nextRun;
		}

		/// <summary>
		/// Gets the first scheduled run time
		/// </summary>
		/// <returns>The next run time</returns>
		public DateTime NextRunTime()
		{
			return NextRunTime(DateTime.Now);
		}
		#endregion methods
	}

	[DataContract]
	public abstract class RCronTaskSpec
	{
		[DataMember(Order = 0)]
		public RCronTaskType TaskType;

		[DataMember(Order = 1)]
		public String ProjectFile;

		[DataMember(Order = 2)]
		public String OutputDirectory;
	}

	[DataContract]
	public class RCronPrintTaskSpec : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String PrintSet { get; set; }

		[DataMember(Order = 11)]
		public String OutputFileName { get; set; }

		//todo: add RunDiff option or break out as command

		public String OutputFilePath
		{
			get
			{
				return String.Format(@"{0}{1}\{2}", OutputDirectory, RCronCanon.TimeStamp, OutputFileName);
			}
		}

		public RCronPrintTaskSpec()
		{
			TaskType = RCronTaskType.Print;
		}
	}

	[DataContract]
	public class RCronExportTaskSpec : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String PrintSet { get; set; }

		[DataMember(Order = 11)]
		public String ExportSetup { get; set; }

		public RCronExportTaskSpec()
		{
			TaskType = RCronTaskType.Export;
		}
	}

	[DataContract]
	public class RCronETransmitTaskSpec : RCronTaskSpec
	{
		public RCronETransmitTaskSpec()
		{
			TaskType = RCronTaskType.ETransmit;
		}
	}

	[DataContract]
	public class RCronCommandTaskSpec : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String CommandName { get; set; }

		public RCronCommandTaskSpec()
		{
			TaskType = RCronTaskType.Command;
		}
	}

	[DataContract]
	public class RCronAuditCompactTaskSpec : RCronTaskSpec
	{
		public RCronAuditCompactTaskSpec()
		{
			TaskType = RCronTaskType.AuditCompact;
		}
	}

	[DataContract]
	public class RCronTestTaskSpec : RCronTaskSpec
	{
		public RCronTestTaskSpec()
		{
			TaskType = RCronTaskType.Test;
		}
	}
}
