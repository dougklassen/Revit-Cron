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
        public Uri TasksFileUri { get; set; }
        [DataMember(Order = 1)]
        public Uri LogDirectoryUri { get; set; }
    }
}
