using System;
using System.Collections.Generic;
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
            Regex cmdRegex = new Regex(@"--\S*");

            String cmd = args
                .Where(s => cmdRegex.IsMatch(s))
                .FirstOrDefault();

            if (null == cmd)
            {
                Console.WriteLine("No command specified");
            }
            else
            {
                switch (cmd)
                {
                    case "newIni":
                        break;
                    case "newTasks":
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
