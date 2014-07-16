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
        IEnumerable<RCronTask> GetRCronTasks();

        void PutRCronTasks(IEnumerable<RCronTask> task);
    }

    public class RCronTasksJsonRepo : IRCronTasksRepo
    {
        private String repoFilePath;

        private RCronTasksJsonRepo() { }

        public RCronTasksJsonRepo(String filePath)
            : this()
        {
            repoFilePath = filePath;
        }

        public IEnumerable<RCronTask> GetRCronTasks()
        {
            List<RCronTask> tasks = null;

            using (FileStream fs = new FileStream(repoFilePath, FileMode.Open))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<RCronTask>));
                tasks = (List<RCronTask>)s.ReadObject(fs);
            }

            return tasks;
        }

        public void PutRCronTasks(IEnumerable<RCronTask> tasks)
        {
            using (FileStream fs = new FileStream(repoFilePath, FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(List<RCronTask>));
                s.WriteObject(fs, tasks);
            }
        }
    }
}