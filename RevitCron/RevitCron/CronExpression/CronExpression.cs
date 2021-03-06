﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron
{
	/// <summary>
	/// A class encapsulating a Cron schedule expression composed of five sub expressions: minutes, hours, days of the month, months, and days of the week
	/// </summary>
	public class CronExpression
	{
		/// <summary>
		/// The minutes of the hour specified by the expression
		/// </summary>
		public CronMinutes Minutes;
		/// <summary>
		/// The hours of the day specified by the expression
		/// </summary>
		public CronHours Hours;
		/// <summary>
		/// The days of the month specified by the expression
		/// </summary>
		public CronDays Days;
		/// <summary>
		/// The months of the year specified by the expression
		/// </summary>
		public CronMonths Months;
		/// <summary>
		/// The days of the week specified by the expression
		/// </summary>
		public CronWeekDays WeekDays;

		private CronExpression() { }

		/// <summary>
		/// Ctor for CronExpression
		/// </summary>
		/// <param name="str">A string representation of the CronExpression to be parsed</param>
		public CronExpression(String str)
			: this()
		{
			String[] subExp = Regex.Split(str, @" ");
			if (subExp.Count() != 5)
			{
				throw new ArgumentException("Cron expression must have five terms");
			}
			try
			{
				Minutes = new CronMinutes(subExp[0]);
				Hours = new CronHours(subExp[1]);
				Days = new CronDays(subExp[2]);
				Months = new CronMonths(subExp[3]);
				WeekDays = new CronWeekDays(subExp[4]);
			}
			catch (ArgumentException exc)
			{
				throw new ArgumentException("Cron expression had invalid terms", exc);
			}
			//todo: check for disallowed day/month combinations
		}

		/// <summary>
		/// Get all run times during the current year
		/// </summary>
		/// <returns>A collection of all run times from January 1st to December 31st of the current year</returns>
		public IEnumerable<DateTime> GetAnnualRunTimes()
		{
			return GetAnnualRunTimes(DateTime.Now.Year);
		}
		
		/// <summary>
		/// Get all run times during a given year
		/// </summary>
		/// <param name="year">The year for which to return run times</param>
		/// <returns>A collection of all run times from January 1st to December 31st of the given year</returns>
		public IEnumerable<DateTime> GetAnnualRunTimes(Int32 year)
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();
			if (!Days.IsWildCard() && WeekDays.IsWildCard())
			{
				runIntervals = CronUtils.GetCartesianProduct(Months.GetRunTimes(), Days.GetRunTimes());
			}
			else if (Days.IsWildCard() && WeekDays.IsWildCard())	//indicates task should be run everyday of the month
			{
				runIntervals = CronUtils.GetCartesianProduct(Months.GetRunTimes(), Days.GetRunTimes());	//if Days is a wildcard, Days.GetRunTimes() returns 1-31
			}
			else if (Days.IsWildCard() && !WeekDays.IsWildCard())
			{
				foreach (var month in Months.Expand())
				{
					var monthInterval = CronUtils.GetMonthTimeSpan(month);
					var weekDayIntervals = WeekDays.GetRunTimes(DateTime.Now.Year, month);

					foreach (var weekDayInterval in weekDayIntervals)
					{
						runIntervals.Add(monthInterval + weekDayInterval);
					}
				}
			}
			else	//neither is a wildcard so use both
			{
				throw new NotImplementedException();
			}
			runIntervals = CronUtils.GetCartesianProduct(runIntervals, Hours.GetRunTimes());
			runIntervals = CronUtils.GetCartesianProduct(runIntervals, Minutes.GetRunTimes()); 

			DateTime yearBeginning = new DateTime(year, 1, 1);
			IEnumerable<DateTime> annualRunTimes = runIntervals
				.Select(i => yearBeginning.Add(i)); //Turns the timespan into a DateTime for this year
			return annualRunTimes;
		}

		public String ToString()
		{
			String cronString = String.Format("{0} {1} {2} {3} {4}", Minutes, Hours, Days, Months, WeekDays);
			return cronString.ToString();
		}
	}
}