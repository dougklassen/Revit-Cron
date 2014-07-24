using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;

using DougKlassen.Revit.Cron.Models;

namespace DougKlassen.Revit.Cron.Repositories
{
    public interface IRCronTasksRepo
    {
        ICollection<RCronTask> GetRCronTasks();

        void PutRCronTasks(ICollection<RCronTask> task);
    }

    public class RCronTasksJsonRepo : IRCronTasksRepo
    {
        private String repoFilePath;

        private RCronTasksJsonRepo() { }

        public RCronTasksJsonRepo(Uri fileUri)
            : this()
        {
            if (fileUri.IsFile)
            {
                repoFilePath = fileUri.LocalPath;
            }
            else
            {
                throw new ArgumentException("Tasks file URI was not a file URI");
            }
        }

        public ICollection<RCronTask> GetRCronTasks()
        {
            ICollection<RCronTask> tasks = null;

            using (FileStream fs = new FileStream(repoFilePath, FileMode.Open))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<RCronTask>));
                tasks = (List<RCronTask>)s.ReadObject(fs);
            }

            return tasks;
        }

        public void PutRCronTasks(ICollection<RCronTask> tasks)
        {
            using (FileStream fs = new FileStream(repoFilePath, FileMode.Create))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<RCronTask>));
                s.WriteObject(fs, tasks);
            }
        }

        public static ICollection<RCronTask> LoadTasks(Uri uri) //convenience method to load tasks from a file
        {
            RCronTasksJsonRepo repo = new RCronTasksJsonRepo(uri);
            return repo.GetRCronTasks();
        }

        public static void WriteTasks(Uri uri, ICollection<RCronTask> tasks)
        {
            RCronTasksJsonRepo repo = new RCronTasksJsonRepo(uri);
            repo.PutRCronTasks(tasks);
        }
    }
}