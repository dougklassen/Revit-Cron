using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron
{
	/// <summary>
	/// A class encapsulating the minutes term of a CronExpression. Minutes are expressed from 0 to 59.
	/// </summary>
	public class CronMinutes
	{
		/// <summary>
		/// Minutes of the hour on which to run from 0 to 59. Null indicates a wildcard value.
		/// </summary>
		private UInt16[] runTimes;
		/// <summary>
		/// Denominator of minutes. Null indicates the minutes term is not denominated
		/// </summary>
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 60 minute values in the range 0 to 59
		/// </summary>
		public Regex seriesRegex = new Regex(@"^([1-5][\d]|[\d])(,([1-5][\d]|[\d])){0,59}$");	//todo: prevent duplicates
		/// <summary>
		/// match a range of minutes in the form 0-59
		/// </summary>
		public Regex rangeRegex = new Regex(@"^(?<s>[1-5][\d]|[\d])-(?<e>[1-5][\d]|[\d])");
		/// <summary>
		/// match an expression denominated by a number in the range 1 to 60
		/// </summary>
		public Regex denominatedMinutesRegex = new Regex(@"^\*\/(?<d>60|[1-5][0-9]|[1-9])$");

		public CronMinutes(String expr)
		{
			if ("*" == expr)
			{
				runTimes = null;
				denominator = null;
			}
			else if (seriesRegex.IsMatch(expr))
			{
				//todo: check for duplicate values
				runTimes = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
				denominator = null;
			}
			else if (rangeRegex.IsMatch(expr))
			{
				Match m = rangeRegex.Match(expr);
				UInt16 start = UInt16.Parse(m.Groups["s"].Value);
				UInt16 end = UInt16.Parse(m.Groups["e"].Value);
				if (start >= end)
				{
					throw new ArgumentException(expr + " is an invalid range");
				}
				List<UInt16> vals = new List<UInt16>();
				for (UInt16 i = start; i <= end; i++)
				{
					vals.Add(i);
				}
				runTimes = vals.ToArray();
				denominator = null;
			}
			else if (denominatedMinutesRegex.IsMatch(expr))
			{
				runTimes = null;
				String dString = denominatedMinutesRegex.Match(expr).Groups["d"].Value;
				denominator = UInt16.Parse(dString);
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid minutes term");
			}
		}
		
		/// <summary>
		/// Get the runtimes represented by the task, represented as minutes past every hour
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runTimes = new List<TimeSpan>();

			return runTimes;
		}

		public override String ToString()
		{
			if (null == runTimes && null == denominator)
			{
				return "*";
			}
			else if (runTimes != null && denominator == null)
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				if (runTimes.IsContiguous())
				{
					exprBuilder.AppendFormat("{0}-{1}", runTimes.Min(), runTimes.Max());
				}
				else
				{
					exprBuilder.Append(runTimes[0]);
					if (runTimes.Count() > 1)
					{
						for (int i = 1; i < runTimes.Count(); i++)
						{
							exprBuilder.AppendFormat(",{0}", runTimes[i]);
						}
					}
					
				}
				return exprBuilder.ToString();
			}
			else if (runTimes == null && denominator != null)
			{
				return "*/" + denominator;
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}
		}
	}

	/// <summary>
	/// A class encapsulating the hours term of a CronExpression. Hours are expressed from 0 to 23, with 0 representing midnite.
	/// </summary>
	public class CronHours
	{
		private UInt16[] runHours;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 24 hour values in the range of 0 to 23
		/// </summary>
		public Regex hourRegex = new Regex(@"^((2[0-3])|(1[\d])|([\d]))(,((2[0-3])|(1[\d])|([\d]))){0,23}$");
		/// <summary>
		/// match an expression denominated by a number between 1 and 24
		/// </summary>
		public Regex denominatedHoursRegex = new Regex(@"^\*\/((2[0-4])|(1[0-9])|[1-9])$");

		public CronHours(String expr)
		{
			if ("*" == expr)
			{
				runHours = null;
				denominator = null;
			}
			else if (hourRegex.IsMatch(expr))
			{
				runHours = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
				denominator = null;
			}
			else if (hourRegex.IsMatch(expr))
			{
				runHours = null;
				String dString = denominatedHoursRegex.Match(expr).Groups['d'].Value;
				denominator = UInt16.Parse(dString);
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid hours term");
			}
		}

		/// <summary>
		/// Get the runtimes represented by the task, represented as hours past midnite
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runTimes = new List<TimeSpan>();

			return runTimes;
		}

		/// <summary>
		/// Return the hours expression in Cron format
		/// </summary>
		/// <returns>The Cron String</returns>
		public override String ToString()
		{
			if (runHours == null && denominator == null)
			{
				return "*";
			}
			else if (runHours != null && denominator == null)
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				exprBuilder.Append(runHours[0]);
				if (runHours.Count() > 1)
				{
					for (int i = 1; i < runHours.Count(); i++)
					{
						exprBuilder.AppendFormat(",{0}", runHours[i]);
					}
				}
				return exprBuilder.ToString();
			}
			else if (runHours == null && denominator != null)
			{
				return "*/" + denominator;
			}
			else
			{
				throw new InvalidOperationException("CronHours object is corrupted");
			}
		}
	}

	/// <summary>
	/// A class encapsulating the days of the month term of a CronExpression. Days are expressed from 1 to 31.
	/// </summary>
	public class CronDays
	{
		private UInt16[] runDays;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 31 days of the month between 1 and 31
		/// </summary>
		public Regex daysRegex = new Regex(@"^(3[01]|[12][\d]|[1-9])(,(3[01]|[12][\d]|[1-9])){0,30}$");
		/// <summary>
		/// match an expression denominated by a number between 1 and 31
		/// </summary>
		public Regex denominatedDaysRegex = new Regex(@"^\*\/(3[01]|[12][\d]|[1-9])$");

		public CronDays(String expr)
		{
			if ("*" == expr)
			{
				runDays = null;
				denominator = null;
			}
			else if (daysRegex.IsMatch(expr))
			{
				runDays = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
				denominator = null;
			}
			else if (daysRegex.IsMatch(expr))
			{
				runDays = null;
				String dString = denominatedDaysRegex.Match(expr).Groups['d'].Value;
				denominator = UInt16.Parse(dString);
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid days term");
			}
		}

		/// <summary>
		/// Get the runtimes represented by the task, represented as days past the first of the month
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runTimes = new List<TimeSpan>();

			return runTimes;
		}

		public override String ToString()
		{
			if (runDays == null && denominator == null)
			{
				return "*";
			}
			else if (runDays != null && denominator == null)
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				exprBuilder.Append(runDays[0]);
				if (runDays.Count() > 1)
				{
					for (int i = 1; i < runDays.Count(); i++)
					{
						exprBuilder.AppendFormat(",{0}", runDays[i]);
					}
				}
				return exprBuilder.ToString();
			}
			else if (runDays == null && denominator != null)
			{
				return "*/" + denominator;
			}
			else
			{
				throw new InvalidOperationException("CronDays object is corrupted");
			}
		}
	}

	/// <summary>
	/// A class encapsulating the months term of a CronExpression. Months are expressed from 1 to 12.
	/// </summary>
	public class CronMonths
	{
		private UInt16[] runMonths;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 12 days of the month between 1 and 12
		/// </summary>
		public Regex monthsRegex = new Regex(@"^(1[0-2]|[1-9])(,(1[0-2]|[1-9])){0,11}$");
		/// <summary>
		/// match an expression denominated by a number between 1 and 12
		/// </summary>
		public Regex denominatedDaysRegex = new Regex(@"^\*\/(1[0-2]|[1-9])$");

		public CronMonths(String expr)
		{
			if ("*" == expr)
			{
				runMonths = null;
				denominator = null;
			}
			else if (monthsRegex.IsMatch(expr))
			{
				runMonths = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
				denominator = null;
			}
			else if (monthsRegex.IsMatch(expr))
			{
				runMonths = null;
				String dString = denominatedDaysRegex.Match(expr).Groups['d'].Value;
				denominator = UInt16.Parse(dString);
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid days term");
			}
		}

		/// <summary>
		/// Get the runtimes represented by the task, represented as months past every new year
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runTimes = new List<TimeSpan>();

			return runTimes;
		}

		public override String ToString()
		{
			if (runMonths == null && denominator == null)
			{
				return "*";
			}
			else if (runMonths != null && denominator == null)
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				exprBuilder.Append(runMonths[0]);
				if (runMonths.Count() > 1)
				{
					for (int i = 1; i < runMonths.Count(); i++)
					{
						exprBuilder.AppendFormat(",{0}", runMonths[i]);
					}
				}
				return exprBuilder.ToString();
			}
			else if (runMonths == null && denominator != null)
			{
				return "*/" + denominator;
			}
			else
			{
				throw new InvalidOperationException("CronMonths object is corrupted");
			}
		}
	}

	/// <summary>
	/// A class encapsulating the day of the week term of a CronExpression. Weekdays are expressed from 0 to 6, with 0 representing Sunday.
	/// </summary>
	public class CronWeekDays
	{
		private UInt16[] runDays;

		/// <summary>
		/// match a list of 1 to 7 days of the week between 0 and 6, 0 equals Sunday
		/// </summary>
		public Regex daysRegex = new Regex(@"^[0-6](,[0-6]){0,6}$");

		public CronWeekDays(String expr)
		{
			if ("*" == expr)
			{
				runDays = null;
			}
			else if (daysRegex.IsMatch(expr))
			{
				runDays = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid days term");
			}
		}

		/// <summary>
		/// Get the runtimes represented by the task, represented as days past every Sunday
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runTimes = new List<TimeSpan>();

			return runTimes;
		}

		public override String ToString()
		{
			if (runDays == null)
			{
				return "*";
			}
			else
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				exprBuilder.Append(runDays[0]);
				if (runDays.Count() > 1)
				{
					for (int i = 1; i < runDays.Count(); i++)
					{
						exprBuilder.AppendFormat(",{0}", runDays[i]);
					}
				}
				return exprBuilder.ToString();
			}
		}
	}
}
