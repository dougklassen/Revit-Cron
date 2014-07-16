using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DougKlassen.Revit.Cron.Models
{
    [DataContract]
    public class RCronTask
    {
        [DataMember(Order = 0)]
        public String Name { get; set; }

        [DataMember(Order = 1)]
        public TaskType Task { get; set; }

        [DataMember(Order = 2)]
        public DateTime LastRun { get; set; }

        [DataMember(Order = 3)]
        public String Schedule { get; set; }
    }

    [DataContract]
    public class rCronPrintTask : RCronTask
    {
        [DataMember(Order = 10)]
        public String PrintSet { get; set; }

        public rCronPrintTask()
        {
            Task = TaskType.Print;
        }
    }

    [DataContract]
    public class rCronExportTask : RCronTask
    {
        [DataMember(Order = 10)]
        public String PrintSet { get; set; }

        [DataMember(Order = 11)]
        public String ExportSetup { get; set; }

        public rCronExportTask()
        {
            Task = TaskType.Export;
        }
    }

    [DataContract]
    public class rCronETransmitTask : RCronTask
    {
        [DataMember(Order = 10)]
        public String OutputDirectory { get; set; }

        public rCronETransmitTask()
        {
            Task = TaskType.ETransmit;
        }
    }

    [DataContract]
    public class rCronCommandTask : RCronTask
    {
        [DataMember(Order = 10)]
        public String CommandName { get; set; }

        public rCronCommandTask()
        {
            Task = TaskType.Command;
        }
    }
}

namespace DougKlassen.Revit.Cron
{
    public enum TaskType
    {
        Print, Export, ETransmit, Command
    }
}
