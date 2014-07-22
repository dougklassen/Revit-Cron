using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Repositories
{
    public interface IRotogravureOptionsRepo
    {
        RotogravureOptions GetRotogravureOptions();

        void PutRotogravureOptions(RotogravureOptions options);
    }

    public class RotogravureOptionsJsonRepo : IRotogravureOptionsRepo
    {
        private String repoFilePath;

        private RotogravureOptionsJsonRepo() { }

        public RotogravureOptionsJsonRepo(String filePath)
            : this()
        {
            repoFilePath = filePath;
        }

        public RotogravureOptions GetRotogravureOptions()
        {
            RotogravureOptions options = null;

            using (FileStream fs = new FileStream(repoFilePath, FileMode.Open))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RotogravureOptions));
                options = (RotogravureOptions)s.ReadObject(fs);
            }

            return options;
        }

        public void PutRotogravureOptions(RotogravureOptions options)
        {
            using(FileStream fs = new FileStream(repoFilePath, FileMode.Create))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RotogravureOptions));
                s.WriteObject(fs, options);
            }
        }
    }
}
