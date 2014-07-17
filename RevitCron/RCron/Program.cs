using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Models;
using DougKlassen.Revit.Cron.Repositories;

namespace RCron
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex cmdRegex = new Regex(@"--(\S*)");

            IEnumerable<String> cmds = args
                .Where(s => cmdRegex.IsMatch(s));

            if (null == cmds.FirstOrDefault())
            {
                Console.WriteLine("No command specified");
            }
            else
            {
                String cmd = cmdRegex.Match(cmds.First()).Groups[1].Value.ToLower();
                switch (cmd)
                {
                    case "newini":
                        Console.WriteLine("newIni command specified");
                        new RotogravureOptionsJsonRepo(RCronFileLocations.OptionsFilePath).PutRotogravureOptions(Dummies.dummyOpts);
                        break;
                    case "newtasks":
                        Console.WriteLine("newTasks command specified");
                        String tasksFileLocation = new RotogravureOptionsJsonRepo(RCronFileLocations.OptionsFilePath)
                            .GetRotogravureOptions()
                            .TasksRepoUri;
                        new RCronTasksJsonRepo(tasksFileLocation).PutRCronTasks(Dummies.dummyTasks);
                        break;
                    default:
                        Console.WriteLine("{0} : command not recognized", cmd);
                        break;
                }
            }
        }


    }
}
