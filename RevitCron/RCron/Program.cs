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
			optionsRepo = new RCronOptionsJsonRepo(new Uri(RCronFileLocations.OptionsFilePath));
			batchRepo = new RCronBatchJsonRepo(new Uri(RCronFileLocations.BatchFilePath));
			scheduleRepo = new RCronScheduleJsonRepo(new Uri(RCronFileLocations.ScheduleFilePath));

			if (args.Length == 0)
			{
				Console.WriteLine("No command specified");
			}
			else
			{
				switch (args[0])
				{
					case "-newoptions":
						CreateOptionsRepo();
						break;
					case "-newbatch":
						Console.WriteLine("created new batch.json");
						batchRepo.PutRCronBatch(Dummies.dummyBatch);
						break;
					case "-testschedule":
						Console.WriteLine("created new test schedule.json");
						scheduleRepo.PutRCronSchedule(Dummies.testSchedule);
						break;
					case "-newschedule":
						Console.WriteLine("created new schedule.json");
						scheduleRepo.PutRCronSchedule(Dummies.dummySchedule);
						break;
					case "-readschedule":
						Console.WriteLine("reading schedule file:");
						var readSched = scheduleRepo.GetRCronSchedule();
						foreach (var item in readSched.Tasks)
						{
							Console.WriteLine(item.Name);
						}
						break;
					case "-timestamp":
						Console.WriteLine(RCronCanon.TimeStamp);
						break;
					case "-uri":
						if (args.Length < 2)
						{
							Console.WriteLine("-uri requires a directory argument");
							break;
						}
						String path = Directory.GetCurrentDirectory() + '\\' + args[1];
						Console.WriteLine("Uri: " + new Uri(path).AbsoluteUri);
						break;
					default:
						Console.WriteLine("{0} : command not recognized", args[0]);
						Console.WriteLine("commands:\n-newoptions\n-newbatch\n-newschedule\n-readschedule\n-testschedule\n-timestamp\n-uri");
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
