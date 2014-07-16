using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DougKlassen.Revit.Cron.Models
{
    [DataContract]
    public class RotogravureOptions
    {
        [DataMember(Order = 0)]
        public String TasksRepoUri { get; set; }
    }
}
