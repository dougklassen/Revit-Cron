using DougKlassen.Revit.Cron;
using DougKlassen.Revit.Cron.Repositories;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RCron
{
	class Program
	{
		static RCronOptionsJsonRepo optionsRepo;
		static RCronScheduleJsonRepo scheduleRepo;
		static RCronBatchJsonRepo batchRepo;

		static void Main(string[] args)
		{
			if (args.Length >= 2)
			{
				if ("-uri" == args[0])
				{
					String path = Directory.GetCurrentDirectory() + '\\' + args[1];
					try
					{
						Console.WriteLine("Uri: " + new Uri(path).AbsoluteUri);
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc.Message);
					}
				}
			}

			Regex cmdRegex = new Regex(@"-(\S*)");

			IEnumerable<String> cmds = args
					.Where(s => cmdRegex.IsMatch(s));

			optionsRepo = new RCronOptionsJsonRepo(new Uri(RCronFileLocations.OptionsFilePath));
			batchRepo = new RCronBatchJsonRepo(new Uri(RCronFileLocations.BatchFilePath));
			scheduleRepo = new RCronScheduleJsonRepo(new Uri(RCronFileLocations.ScheduleFilePath));

			if (null == cmds.FirstOrDefault())
			{
				Console.WriteLine("No command specified");
			}
			else
			{
				String cmd = cmdRegex.Match(cmds.First()).Groups[1].Value.ToLower();
				switch (cmd)
				{
					case "newoptions":
						CreateOptionsRepo();
						break;
					case "newbatch":
						Console.WriteLine("created new batch.json");
						batchRepo.PutRCronBatch(Dummies.dummyBatch);
						break;
					case "testschedule":
						Console.WriteLine("created new test schedule.json");
						scheduleRepo.PutRCronSchedule(Dummies.testSchedule);
						break;
					case "newschedule":
						Console.WriteLine("created new schedule.json");
						scheduleRepo.PutRCronSchedule(Dummies.dummySchedule);
						break;
					case "timestamp":
						Console.WriteLine(RCronCanon.TimeStamp);
						break;
					default:
						Console.WriteLine("{0} : command not recognized", cmd);
						Console.WriteLine("commands:\n-newoptions\n-newbatch\n-testschedule\n-timestamp\n-uri");
						break;
				}
			}
		}

		static void CreateOptionsRepo()
		{
			Console.WriteLine("created new options.json");
			optionsRepo.PutRCronOptions(Dummies.dummyOpts);
		}
	}
}
