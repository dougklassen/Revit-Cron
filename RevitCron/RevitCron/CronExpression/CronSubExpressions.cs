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
		public Regex denominatedRegex = new Regex(@"^\*\/(?<d>60|[1-5][0-9]|[1-9])$");

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
			else if (denominatedRegex.IsMatch(expr))
			{
				runTimes = null;
				String dString = denominatedRegex.Match(expr).Groups["d"].Value;
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
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			if (runTimes == null && denominator == null)
			{
				for (int i = 0; i < 60; i++)
				{
					runIntervals.Add(TimeSpan.FromMinutes(i));
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(TimeSpan.FromMinutes(time));
				}
			}
			else if (runTimes == null && denominator != null)
			{
				Double denominatedInterval = 60 / (Double)denominator;
				for (int i = 0; i < denominator; i++)
				{
					runIntervals.Add(TimeSpan.FromMinutes(Math.Floor(denominatedInterval * i)));
				}
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor are't
		/// idempotent because contiguous series may be reduced to dash notation
		/// </summary>
		/// <returns>The Cron string</returns>
		public override String ToString()
		{
			if (null == runTimes && null == denominator)
			{
				return "*";
			}
			else if (runTimes != null && denominator == null)
			{
				return runTimes.GetSeriesCronString();
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
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 24 hour values in the range of 0 to 23
		/// </summary>
		public Regex seriesRegex = new Regex(@"^(2[0-3]|1[\d]|[\d])(,(2[0-3]|1[\d]|[\d])){0,23}$");
		/// <summary>
		/// match a range of hours in the form 0-23
		/// </summary>
		public Regex rangeRegex = new Regex(@"^(?<s>2[0-3]|1[\d]|[\d])-(?<e>2[0-3]|1[\d]|[\d])");
		/// <summary>
		/// match an expression denominated by a number between 1 and 24
		/// </summary>
		public Regex denominatedRegex = new Regex(@"^\*\/(?<d>2[0-4]|1[\d]|[1-9])$");

		public CronHours(String expr)
		{
			if ("*" == expr)
			{
				runTimes = null;
				denominator = null;
			}
			else if (seriesRegex.IsMatch(expr))
			{
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
			else if (denominatedRegex.IsMatch(expr))
			{
				runTimes = null;
				String dString = denominatedRegex.Match(expr).Groups["d"].Value;
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
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			if (runTimes == null && denominator == null)
			{
				for (int i = 0; i < 24; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(i));
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(TimeSpan.FromHours(time));
				}
			}
			else if (runTimes == null && denominator != null)
			{
				Double denominatedInterval = 60 / (Double)denominator;
				for (int i = 0; i < denominator; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(Math.Floor(denominatedInterval * i)));
				}
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor are't
		/// idempotent because contiguous series may be reduced to dash notation
		/// </summary>
		/// <returns>The Cron string</returns>
		public override String ToString()
		{
			if (runTimes == null && denominator == null)
			{
				return "*";
			}
			else if (runTimes != null && denominator == null)
			{
				return runTimes.GetSeriesCronString();
			}
			else if (runTimes == null && denominator != null)
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
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 31 days of the month between 1 and 31
		/// </summary>
		public Regex seriesRegex = new Regex(@"^(3[01]|[12][\d]|[1-9])(,(3[01]|[12][\d]|[1-9])){0,30}$");
		/// <summary>
		/// match a range of days in the form 1-31
		/// </summary>
		public Regex rangeRegex = new Regex(@"^(?<s>3[01]|[12][\d]|[1-9])-(?<e>3[01]|[12][\d]|[1-9])");
		/// <summary>
		/// match an expression denominated by a number between 1 and 31
		/// </summary>
		public Regex denominatedRegex = new Regex(@"^\*\/(?<d>3[01]|[12][\d]|[1-9])$");

		public CronDays(String expr)
		{
			if ("*" == expr)
			{
				runTimes = null;
				denominator = null;
			}
			else if (seriesRegex.IsMatch(expr))
			{
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
			else if (denominatedRegex.IsMatch(expr))
			{
				runTimes = null;
				String dString = denominatedRegex.Match(expr).Groups["d"].Value;
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
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			if (runTimes == null && denominator == null)
			{
				for (int i = 0; i < 31; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(24 * i));
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(TimeSpan.FromHours(24 * time));
				}
			}
			else if (runTimes == null && denominator != null)
			{
				Double denominatedInterval = 60 / (Double)denominator;
				for (int i = 0; i < denominator; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(24 * Math.Floor(denominatedInterval * i)));
				}
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor are't
		/// idempotent because contiguous series may be reduced to dash notation
		/// </summary>
		/// <returns>The Cron string</returns>
		public override String ToString()
		{
			if (runTimes == null && denominator == null)
			{
				return "*";
			}
			else if (runTimes != null && denominator == null)
			{
				return runTimes.GetSeriesCronString();
			}
			else if (runTimes == null && denominator != null)
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
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 12 days of the month between 1 and 12
		/// </summary>
		public Regex seriesRegex = new Regex(@"^(1[0-2]|[1-9])(,(1[0-2]|[1-9])){0,11}$");
		/// <summary>
		/// match a range of months in the form 1-12
		/// </summary>
		public Regex rangeRegex = new Regex(@"^(?<s>1[0-2]|[1-9])-(?<e>1[0-2]|[1-9])");
		/// <summary>
		/// match an expression denominated by a number between 1 and 12
		/// </summary>
		public Regex denominatedRegex = new Regex(@"^\*\/(?<d>1[0-2]|[1-9])$");

		public CronMonths(String expr)
		{
			if ("*" == expr)
			{
				runTimes = null;
				denominator = null;
			}
			else if (seriesRegex.IsMatch(expr))
			{
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
			else if (denominatedRegex.IsMatch(expr))
			{
				runTimes = null;
				String dString = denominatedRegex.Match(expr).Groups["d"].Value;
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
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			if (runTimes == null && denominator == null)
			{
				for (int i = 0; i < 12; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(31 * 24 * i));
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(TimeSpan.FromHours(31 * 24 * time));
				}
			}
			else if (runTimes == null && denominator != null)
			{
				Double denominatedInterval = 60 / (Double)denominator;
				for (int i = 0; i < denominator; i++)
				{
					runIntervals.Add(TimeSpan.FromHours(31 * 24 * Math.Floor(denominatedInterval * i)));
				}
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor are't
		/// idempotent because contiguous series may be reduced to dash notation
		/// </summary>
		/// <returns>The Cron string</returns>
		public override String ToString()
		{
			if (runTimes == null && denominator == null)
			{
				return "*";
			}
			else if (runTimes != null && denominator == null)
			{
				return runTimes.GetSeriesCronString();
			}
			else if (runTimes == null && denominator != null)
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
		private UInt16[] runTimes;

		/// <summary>
		/// match a list of 1 to 7 days of the week between 0 and 6, with 0 equal to Sunday
		/// </summary>
		public Regex seriesRegex = new Regex(@"^[0-6](,[0-6]){0,6}$");
		/// <summary>
		/// match a range of week days in the form 0-6
		/// </summary>
		public Regex rangeRegex = new Regex(@"^(?<s>[0-6])-(?<e>[0-6])");

		public CronWeekDays(String expr)
		{
			if ("*" == expr)
			{
				runTimes = null;
			}
			else if (seriesRegex.IsMatch(expr))
			{
				runTimes = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
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

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor are't
		/// idempotent because contiguous series may be reduced to dash notation
		/// </summary>
		/// <returns>The Cron string</returns>
		public override String ToString()
		{
			if (runTimes == null)
			{
				return "*";
			}
			else
			{
				return runTimes.GetSeriesCronString();
			}
		}
	}
}
