﻿using DougKlassen.Revit.Cron;
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

			CheckDirectories();

			if (args.Length == 0)
			{
				Console.WriteLine("No command specified");
				WriteHelp();
			}
			else
			{
				switch (args[0])
				{
					case "-help":
						WriteHelp();
						break;
					case "-newoptions":
						CreateOptionsRepo();
						break;
					case "-newbatch":
						Console.WriteLine("created new batch.json");
						batchRepo.PutRCronBatch(Dummies.dummyBatch);
						break;
					case "-readbatch":
						ReadBatch();
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
						ReadSchedule();
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
						WriteHelp();
						break;
				}
			}
		}

		static void CreateOptionsRepo()
		{
			optionsRepo.PutRCronOptions(Dummies.dummyOpts);
			Console.WriteLine("created new options.json");
		}

		static void ReadBatch()
		{
			if (!File.Exists(RCronFileLocations.BatchFilePath))
			{
				Console.WriteLine("batch file not found");
				return;
			}
			Console.WriteLine("reading batch file");
			var readBatch = batchRepo.GetRCronBatch();
			Console.WriteLine("start time: " + readBatch.StartTime);
			Console.WriteLine("end time: " + readBatch.EndTime);
			Console.WriteLine("number of tasks: " + readBatch.TaskSpecs.Count);
			foreach (var item in readBatch.TaskSpecs)
			{
				Console.WriteLine(item.Key);
			}
		}

		static void ReadSchedule()
		{
			if (!File.Exists(RCronFileLocations.ScheduleFilePath))
			{
				Console.WriteLine("schedule file not found");
				return;
			}
			Console.WriteLine("reading schedule file:");
			var readSched = scheduleRepo.GetRCronSchedule();
			foreach (var item in readSched.Tasks)
			{
				Console.WriteLine(item.Name);
			}
			return;
		}

		static void WriteHelp()
		{
			Console.WriteLine("commands:\n-newoptions\n-newbatch\n-newschedule\n-readschedule\n-testschedule\n-timestamp\n-uri");
			return;
		}

		static void CheckDirectories()
		{
			if (!Directory.Exists(RCronFileLocations.ResourcesDirectoryPath))
			{
				Directory.CreateDirectory(RCronFileLocations.ResourcesDirectoryPath);
				Console.WriteLine("created " + RCronFileLocations.ResourcesDirectoryPath);
			}

			if (!Directory.Exists(RCronFileLocations.LogDirectoryPath))
			{
				Directory.CreateDirectory(RCronFileLocations.LogDirectoryPath);
				Console.WriteLine("created " + RCronFileLocations.LogDirectoryPath);
			}
		}
	}
}
