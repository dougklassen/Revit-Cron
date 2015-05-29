using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron
{
	public abstract class CronSubExpression
	{
		/// <summary>
		/// Get the runtimes represented by the task, represented as a timespan past the next largest increment of time
		/// </summary>
		/// <returns>A collection of run times</returns>
		public abstract IList<TimeSpan> GetRunTimes();

		/// <summary>
		/// Returns whether the sub-expression is assigned a wildcard value
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public abstract Boolean IsWildCard();

		/// <summary>
		/// Expands a term into the set of all run times it represents within the the next largest unit of time
		/// </summary>
		/// <returns>An array of integers representing run times</returns>
		public abstract IEnumerable<UInt16> Expand();

		/// <summary>
		/// Overload for calculating the Cartesian product of two terms
		/// </summary>
		/// <param name="larger">The term representing the larger interval</param>
		/// <param name="smaller">The term representing the smaller term</param>
		/// <returns>The product of the two terms</returns>
		public static IEnumerable<TimeSpan> operator *(CronSubExpression larger, CronSubExpression smaller)
		{
			return CronUtils.GetCartesianProduct(larger.GetRunTimes(), smaller.GetRunTimes());
		}
	}

	/// <summary>
	/// A class encapsulating the minutes term of a CronExpression. Minutes are expressed from 0 to 59.
	/// </summary>
	public class CronMinutes : CronSubExpression
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
		/// Match a list of 1 to 60 minute values in the range 0 to 59
		/// </summary>
		public static readonly Regex seriesRegex = new Regex(@"^([1-5][\d]|[\d])(,([1-5][\d]|[\d])){0,59}$");	//todo: prevent duplicates

		/// <summary>
		/// Match a range of minutes in the form 0-59
		/// </summary>
		public static readonly Regex rangeRegex = new Regex(@"^(?<s>[1-5][\d]|[\d])-(?<e>[1-5][\d]|[\d])");

		/// <summary>
		/// Match an expression denominated by a number in the range 1 to 60
		/// </summary>
		public static readonly Regex denominatedRegex = new Regex(@"^\*\/(?<d>60|[1-5][0-9]|[1-9])$");

		/// <summary>
		/// Ctor based on parsing a string representing the minutes term of a Cron expression
		/// </summary>
		/// <param name="expr">A string representing the minutes term of a Cron expression</param>
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
		/// <returns>A collection of run times</returns>
		public override IList<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			foreach (UInt16 time in this.Expand())
			{
				runIntervals.Add(TimeSpan.FromMinutes(time));
			}

			return runIntervals;
		}

		/// <summary>
		/// Indicates whether the term is expressed as a wild card in the CronExpressiob
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public override Boolean IsWildCard()
		{
			return (runTimes == null && denominator == null);
		}

		/// <summary>
		/// Expands an expression into a complete set of run times, expressed as minutes past the beginning of the hour
		/// </summary>
		/// <returns>An array of integers representing run times</returns>
		public override IEnumerable<UInt16> Expand()
		{
			List<UInt16> runIntervals = new List<UInt16>();

			if (runTimes == null && denominator == null)	//wildcard
			{
				for (UInt16 i = 0; i < 60; i++)
				{
					runIntervals.Add(i);
				}
			}
			else if (runTimes != null && denominator == null)	//series or range
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(time);
				}
			}
			else if (runTimes == null && denominator != null)	//denominated
			{
				for (UInt16 i = 0; i < 60; i += denominator.Value)	//count by the step for every value within one hour
				{
					runIntervals.Add(i);
				}
			}
			else
			{
				throw new InvalidOperationException("CronMinutes object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Get a string in canonical Cron format. Round trip calls through the constructor aren't
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
	public class CronHours : CronSubExpression
	{
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// Match a list of 1 to 24 hour values in the range of 0 to 23
		/// </summary>
		public static readonly Regex seriesRegex = new Regex(@"^(2[0-3]|1[\d]|[\d])(,(2[0-3]|1[\d]|[\d])){0,23}$");

		/// <summary>
		/// Match a range of hours in the form 0-23
		/// </summary>
		public static readonly Regex rangeRegex = new Regex(@"^(?<s>2[0-3]|1[\d]|[\d])-(?<e>2[0-3]|1[\d]|[\d])");

		/// <summary>
		/// Match an expression denominated by a number between 1 and 24
		/// </summary>
		public static readonly Regex denominatedRegex = new Regex(@"^\*\/(?<d>2[0-4]|1[\d]|[1-9])$");

		/// <summary>
		/// Ctor based on parsing a string representing the hours term of a Cron expression
		/// </summary>
		/// <param name="expr">A string representing the hours term of a Cron expression</param>
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
		public override IList<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			foreach (UInt16 time in this.Expand())
			{
				runIntervals.Add(TimeSpan.FromHours(time));
			}

			return runIntervals;
		}

		/// <summary>
		/// Indicates whether the term is expressed as a wild card in the CronExpressiob
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public override Boolean IsWildCard()
		{
			return (runTimes == null && denominator == null);
		}

		/// <summary>
		/// Expands an expression into a complete set of run times, expressed as hours past midnite
		/// </summary>
		/// <returns>An array of integers representing run times</returns>
		public override IEnumerable<UInt16> Expand()
		{
			List<UInt16> runIntervals = new List<UInt16>();

			if (runTimes == null && denominator == null)
			{
				for (UInt16 i = 0; i < 24; i++)
				{
					runIntervals.Add(i);
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(time);
				}
			}
			else if (runTimes == null && denominator != null)
			{
				for (UInt16 i = 0; i < 24; i += denominator.Value)
				{
					runIntervals.Add(i);
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
	/// A class encapsulating the days of the month term of a CronExpression. Days are expressed from 1 to 31, with 1 representing the 1st of the month.
	/// </summary>
	public class CronDays : CronSubExpression
	{
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// Match a list of 1 to 31 days of the month between 1 and 31
		/// </summary>
		public static readonly Regex seriesRegex = new Regex(@"^(3[01]|[12][\d]|[1-9])(,(3[01]|[12][\d]|[1-9])){0,30}$");

		/// <summary>
		/// Match a range of days in the form 1-31
		/// </summary>
		public static readonly Regex rangeRegex = new Regex(@"^(?<s>3[01]|[12][\d]|[1-9])-(?<e>3[01]|[12][\d]|[1-9])");

		/// <summary>
		/// Match an expression denominated by a number between 1 and 31
		/// </summary>
		public static readonly Regex denominatedRegex = new Regex(@"^\*\/(?<d>3[01]|[12][\d]|[1-9])$");

		/// <summary>
		/// Ctor based on parsing a string representing the days term of a Cron expression
		/// </summary>
		/// <param name="expr">A string representing the days term of a Cron expression</param>
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
		public override IList<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			foreach (UInt16 time in Expand())
			{
				runIntervals.Add(TimeSpan.FromDays(time));
			}

			return runIntervals;
		}

		/// <summary>
		/// Expands an expression into a complete set of run times, expressed as days past the first of the month
		/// </summary>
		/// <returns>An array of integers representing run times</returns>
		public override IEnumerable<ushort> Expand()
		{
			List<UInt16> runIntervals = new List<UInt16>();

			if (runTimes == null && denominator == null)
			{
				for (UInt16 i = 0; i < 31; i++)
				{
					runIntervals.Add(i);
				}
			}
			else if (runTimes != null && denominator == null)
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(time);
				}
			}
			else if (runTimes == null && denominator != null)
			{
				for (UInt16 i = 0; i < 31; i += denominator.Value)	//todo: need to prevent adding the 31st if it doesn't exist in month
				{
					runIntervals.Add(i);
				}
			}
			else
			{
				throw new InvalidOperationException("CronDays object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Indicates whether the term is expressed as a wild card in the CronExpressiob
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public override Boolean IsWildCard()
		{
			return (runTimes == null && denominator == null);
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
	/// A class encapsulating the months term of a CronExpression. Months are numbered from 1 to 12, with 1 representing January.
	/// </summary>
	public class CronMonths : CronSubExpression
	{
		private UInt16[] runTimes;
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 12 days of the month between 1 and 12
		/// </summary>
		public static readonly Regex seriesRegex = new Regex(@"^(1[0-2]|[1-9])(,(1[0-2]|[1-9])){0,11}$");

		/// <summary>
		/// match a range of months in the form 1-12
		/// </summary>
		public static readonly Regex rangeRegex = new Regex(@"^(?<s>1[0-2]|[1-9])-(?<e>1[0-2]|[1-9])");

		/// <summary>
		/// match an expression denominated by a number between 1 and 12
		/// </summary>
		public static readonly Regex denominatedRegex = new Regex(@"^\*\/(?<d>1[0-2]|[1-9])$");

		/// <summary>
		/// Ctor based on parsing a string representing the months term of a Cron expression
		/// </summary>
		/// <param name="expr">A string representing the months term of a Cron expression</param>
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
		public override IList<TimeSpan> GetRunTimes()
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			foreach (UInt16 time in this.Expand())
			{
				runIntervals.Add(TimeSpan.FromDays(31 * time));
			}

			return runIntervals;
		}

		/// <summary>
		/// Expands an expression into a complete set of run times, expressed as months of the year
		/// </summary>
		/// <returns>An array of integers representing run times</returns>
		public override IEnumerable<ushort> Expand()
		{
			List<UInt16> runIntervals = new List<UInt16>();

			if (runTimes == null && denominator == null)	//wildcard
			{
				for (UInt16 i = 0; i < 12; i++)
				{
					runIntervals.Add(i);
				}
			}
			else if (runTimes != null && denominator == null)	//series or range
			{
				foreach (UInt16 time in runTimes)
				{
					runIntervals.Add(time);
				}
			}
			else if (runTimes == null && denominator != null)	//denominated
			{
				for (UInt16 i = 0; i < 12; i += denominator.Value)
				{
					runIntervals.Add(i);
				}
			}
			else
			{
				throw new InvalidOperationException("CronMonths object is corrupted");
			}

			return runIntervals;
		}

		/// <summary>
		/// Indicates whether the term is expressed as a wild card in the CronExpressiob
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public override Boolean IsWildCard()
		{
			return (runTimes == null && denominator == null);
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
	/// A class encapsulating the day of the week term of a CronExpression. Weekdays are numbered from 0 to 6, with 0 representing Sunday.
	/// </summary>
	/// <remarks>GetRunTimes() is specific to a given month and year</remarks>
	public class CronWeekDays : CronSubExpression
	{
		private UInt16[] runTimes = new UInt16[0];	//initialize as an empty array

		/// <summary>
		/// match a list of 1 to 7 days of the week between 0 and 6, with 0 equal to Sunday
		/// </summary>
		public static readonly Regex seriesRegex = new Regex(@"^[0-6](,[0-6]){0,6}$");

		/// <summary>
		/// match a range of week days in the form 0-6
		/// </summary>
		public static readonly Regex rangeRegex = new Regex(@"^(?<s>[0-6])-(?<e>[0-6])");

		/// <summary>
		/// Create an instance of CronWeekDays by parsing the sub term of the Cron expression representing weekdays
		/// </summary>
		/// <param name="expr">A string representing the minutes term of a Cron expression</param>
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
		/// Get the runtimes represented by the task, represented as days past the first of the month for a the current month
		/// </summary>
		/// <returns></returns>
		public override IList<TimeSpan> GetRunTimes()
		{
			return GetRunTimes(DateTime.Now.Year, DateTime.Now.Month).ToList();
		}

		/// <summary>
		/// Get the runtimes for the task, represented as timespan past the first of the month for a specific month. Each day within the month matching that day will be returned
		/// </summary>
		/// <returns>A collection of TimeSpans representing each day within the specified month that falls on the specified WeekDays</returns>
		public IEnumerable<TimeSpan> GetRunTimes(Int32 year, Int32 month)
		{
			List<TimeSpan> runIntervals = new List<TimeSpan>();

			for (Int32 i = 1; i < DateTime.DaysInMonth(year, month); i++)
			{
				DateTime dayToCheck = new DateTime(year, month, i); //generate a DateTime for each day of the month
				if (this.Expand().Contains((UInt16)dayToCheck.DayOfWeek)) //if the DayOfWeek for the DateTime is one of the specified WeekDays,
				{
					runIntervals.Add(TimeSpan.FromDays(i)); //include it in the set
				}
			}

			return runIntervals;
		}

		/// <summary>
		/// Indicates whether the term is expressed as a wild card in the CronExpressiob
		/// </summary>
		/// <returns>Whether the expression is a wildcard</returns>
		public override Boolean IsWildCard()
		{
			return (runTimes == null);
		}

		/// <summary>
		/// An expanded list of days of the week represented by the WeekDays expression
		/// </summary>
		/// <returns>An array of integers representing days of the week, with 0 equals Sunday</returns>
		public override IEnumerable<ushort> Expand()
		{
			if (null == runTimes)	//wildcard expression
			{
				UInt16[] everyDay = new UInt16[31];
				for (UInt16 i = 0; i < everyDay.Length; i++)
				{
					everyDay[i] = i;
				}
				return everyDay;	//return an array specifying every day of the month
			}
			else
			{
				return runTimes;
			}
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
