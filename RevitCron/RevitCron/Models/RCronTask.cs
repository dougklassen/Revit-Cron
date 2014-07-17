using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
    [DataContract]
    [KnownType(typeof(RCronPrintTask))]
    [KnownType(typeof(RCronExportTask))]
    [KnownType(typeof(RCronETransmitTask))]
    [KnownType(typeof(RCronCommandTask))]
    public class RCronTask
    {
        [DataMember(Order = 0)]
        public String Name { get; set; }

        [DataMember(Order = 1)]
        public DateTime LastRun { get; set; }

        [DataMember(Order = 2)]
        public String Schedule { get; set; }

        [DataMember(Order = 3)]
        public RCronTaskInfo TaskInfo { get; set; }
    }

    [DataContract]
    public class RCronTaskInfo
    {
        [DataMember(Order = 0)]
        public TaskType Task;

        [DataMember(Order = 1)]
        public String OutputDirectory;
    }

    [DataContract]
    public class RCronPrintTask : RCronTaskInfo
    {
        [DataMember(Order = 10)]
        public String PrintSet { get; set; }

        public RCronPrintTask()
        {
            Task = TaskType.Print;
        }
    }

    [DataContract]
    public class RCronExportTask : RCronTaskInfo
    {
        [DataMember(Order = 10)]
        public String PrintSet { get; set; }

        [DataMember(Order = 11)]
        public String ExportSetup { get; set; }

        public RCronExportTask()
        {
            Task = TaskType.Export;
        }
    }

    [DataContract]
    public class RCronETransmitTask : RCronTaskInfo
    {
        public RCronETransmitTask()
        {
            Task = TaskType.ETransmit;
        }
    }

    [DataContract]
    public class RCronCommandTask : RCronTaskInfo
    {
        [DataMember(Order = 10)]
        public String CommandName { get; set; }

        public RCronCommandTask()
        {
            Task = TaskType.Command;
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
