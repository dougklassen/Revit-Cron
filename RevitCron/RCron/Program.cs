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
		static void Main(string[] args)
		{
			if (args.Length >= 2)
			{
				if ("--uri" == args[0])
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

			Regex cmdRegex = new Regex(@"--(\S*)");

			IEnumerable<String> cmds = args
					.Where(s => cmdRegex.IsMatch(s));

			RCronOptionsJsonRepo RCronOptionsRepo = new RCronOptionsJsonRepo(new Uri(RCronFileLocations.OptionsFilePath));

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
						Console.WriteLine("created new options.json");
						RCronOptionsRepo.PutRCronOptions(Dummies.dummyOpts);
						break;
					case "newbatch":
						Console.WriteLine("created new batch.json");
						Uri tasksFileUri = RCronOptionsRepo
								.GetRCronOptions()
								.BatchFileUri;
						new RCronBatchJsonRepo(tasksFileUri).PutRCronBatch(Dummies.dummyBatch);
						break;
					case "newschedule":
						throw new NotImplementedException();
						break;
					case "timestamp":
						Console.WriteLine(RCronCanon.TimeStamp);
						break;
					case "uri":
						break;
					default:
						Console.WriteLine("{0} : command not recognized", cmd);
						break;
				}
			}
		}
	}
}
