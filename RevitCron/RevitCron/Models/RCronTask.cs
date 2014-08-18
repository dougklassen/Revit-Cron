using System;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
	[DataContract]
	[KnownType(typeof(RCronPrintTaskInfo))]
	[KnownType(typeof(RCronExportTaskInfo))]
	[KnownType(typeof(RCronETransmitTaskInfo))]
	[KnownType(typeof(RCronCommandTaskInfo))]
	public class RCronTask
	{
		#region Properties
		/// <summary>
		/// The unique name of the task
		/// </summary>
		[DataMember(Order = 0)]
		public String Name { get; set; }

		/// <summary>
		/// The priority of a task in a batch when queued in a batch, highest first
		/// </summary>
		[DataMember(Order = 10)]
		public Int32 Priority { get; set; }	//to control where the task falls in the batch when queued with other tasks

		/// <summary>
		/// Whether to run the task immediately if it wasn't run at the last scheduled run time
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
		/// The next scheduled run time for the task, or null if not scheduled
		/// </summary>
		[DataMember]
		public DateTime? NextRun {get;set;}


		[DataMember(Order = 10)]
		public RCronTaskSpec TaskInfo { get; set; } //to facilitate serialization, subclassed members belong to a member class
		#endregion Properties

		public Boolean IsDueToRun
		{
			get
			{
				Boolean dueToRun = false;
				//todo: determine if task should run

				//if there's a star, add a grace period to next smallest increment
				return dueToRun;
			}
		}

		public DateTime NextRunTime()
		{
			DateTime nextRun = DateTime.MinValue;

			return nextRun;
		}
	}

	[DataContract]
	public abstract class RCronTaskSpec
	{
		[DataMember(Order = 0)]
		public TaskType TaskType;

		[DataMember(Order = 1)]
		public String ProjectFile;

		[DataMember(Order = 2)]
		public String OutputDirectory;
	}

	[DataContract]
	public class RCronPrintTaskInfo : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String PrintSet { get; set; }

		[DataMember(Order = 11)]
		public String OutputFileName { get; set; }

		//todo: add RunDiff option

		public String OutputFilePath
		{
			get
			{
				return String.Format(@"{0}{1}\{2}", OutputDirectory, RCronCanon.TimeStamp, OutputFileName);
			}
		}

		public RCronPrintTaskInfo()
		{
			TaskType = TaskType.Print;
		}
	}

	[DataContract]
	public class RCronExportTaskInfo : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String PrintSet { get; set; }

		[DataMember(Order = 11)]
		public String ExportSetup { get; set; }

		public RCronExportTaskInfo()
		{
			TaskType = TaskType.Export;
		}
	}

	[DataContract]
	public class RCronETransmitTaskInfo : RCronTaskSpec
	{
		public RCronETransmitTaskInfo()
		{
			TaskType = TaskType.ETransmit;
		}
	}

	[DataContract]
	public class RCronCommandTaskInfo : RCronTaskSpec
	{
		[DataMember(Order = 10)]
		public String CommandName { get; set; }

		public RCronCommandTaskInfo()
		{
			TaskType = TaskType.Command;
		}
	}
}
