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
		public RCronTaskInfo TaskInfo { get; set; } //to facilitate serialization, subclassed members belong to a member class

		[DataMember(Order = 4)]
		public Int32 Priority { get; set; }	//to control where the task fall in the queue when multiple tasks are added
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

//todo: use string keys for enum
namespace DougKlassen.Revit.Cron
{
	public enum TaskType
	{
		Print, Export, ETransmit, Command
	}
}
