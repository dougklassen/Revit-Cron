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
        private Uri repoFileUri;

        private RotogravureOptionsJsonRepo() { }

        public RotogravureOptionsJsonRepo(Uri uri)
            : this()
        {
            repoFileUri = uri;
        }

        public RotogravureOptions GetRotogravureOptions()
        {
            RotogravureOptions options = null;

            using (FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Open))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RotogravureOptions));
                options = (RotogravureOptions)s.ReadObject(fs);
            }

            return options;
        }

        public void PutRotogravureOptions(RotogravureOptions options)
        {
            using(FileStream fs = new FileStream(repoFileUri.LocalPath, FileMode.Create))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(RotogravureOptions));
                s.WriteObject(fs, options);
            }
        }

        public static RotogravureOptions LoadOptions(Uri uri) //convenience method to load options from file
        {
            RotogravureOptionsJsonRepo repo = new RotogravureOptionsJsonRepo(uri);
            return repo.GetRotogravureOptions();
        }

        public static void WriteOptions(Uri uri, RotogravureOptions options) //convenience method to write options to file
        {
            RotogravureOptionsJsonRepo repo = new RotogravureOptionsJsonRepo(uri);
            repo.PutRotogravureOptions(options);
        }
    }
}
