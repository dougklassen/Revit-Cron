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
		[DataMember(Order = 0)]
		public String Name { get; set; }

		[DataMember(Order = 1)]
		public DateTime LastRun { get; set; }

		[DataMember(Order = 2)]
		public String Schedule { get; set; }

		[DataMember(Order = 3)]
		public Int32 Priority { get; set; }	//to control where the task fall in the queue when multiple tasks are added

		//todo: add next scheduled run time as nullable DateTime

		//todo: add RunIfMissed option

		[DataMember(Order = 4)]
		public RCronTaskInfo TaskInfo { get; set; } //to facilitate serialization, subclassed members belong to a member class

		public Boolean IsDueToRun
		{
			get
			{
				Boolean runNow = false;
				//todo: determine if task should run

				//if there's a star, add a grace period to next smallest increment
				return runNow;
			}
		}

		/// <summary>
		/// Returns a sanitized version of a task for use in serialization to a batch file
		/// </summary>
		/// <returns>An abbreviated version of the task</returns>
		public RCronTask GetBatchVersion()
		{
			RCronTask bv = new RCronTask();
			bv.Name = this.Name + "-run";
			bv.LastRun = DateTime.MinValue;
			bv.Schedule = null;
			bv.TaskInfo = this.TaskInfo;

			return bv;
		}
	}

	[DataContract]
	public class RCronTaskInfo
	{
		[DataMember(Order = 0)]
		public TaskType TaskType;

		[DataMember(Order = 1)]
		public String ProjectFile;

		[DataMember(Order = 2)]
		public String OutputDirectory;
	}

	[DataContract]
	public class RCronPrintTaskInfo : RCronTaskInfo
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
	public class RCronExportTaskInfo : RCronTaskInfo
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
	public class RCronETransmitTaskInfo : RCronTaskInfo
	{
		public RCronETransmitTaskInfo()
		{
			TaskType = TaskType.ETransmit;
		}
	}

	[DataContract]
	public class RCronCommandTaskInfo : RCronTaskInfo
	{
		[DataMember(Order = 10)]
		public String CommandName { get; set; }

		public RCronCommandTaskInfo()
		{
			TaskType = TaskType.Command;
		}
	}
}
