using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DougKlassen.Revit.Cron.Models
{
	public class CronExpression
	{
		public CronExpression()
		{

		}

		public CronExpression(String str)
		{
			//todo: check for disallowed day/month combinations
		}

		public String ToString()
		{
			StringBuilder cronString = new StringBuilder(String.Empty);

			return cronString.ToString();
		}
	}

	/// <summary>
	/// A class encapsulating the minutes term of a CronExpression
	/// </summary>
	public class CronMinutes
	{
		/// <summary>
		/// Minutes of the hour on which to run from 0 to 59. Null indicates a wildcard value.
		/// </summary>
		private UInt16[] runMinutes;
		/// <summary>
		/// Denominator of minutes. Null indicates the minutes term is not denominated
		/// </summary>
		private UInt16? denominator;

		/// <summary>
		/// match a list of 1 to 60 minute values in the range 0 to 59
		/// </summary>
		public Regex minutesRegex = new Regex(@"^(([1-5][\d])|([\d]))(,(([1-5][\d])|([\d]))){0,59}$");	//todo: prevent duplicates
		/// <summary>
		/// match an expression denominated by a number in the range 1 to 60
		/// </summary>
		public Regex denominatedMinutesRegex = new Regex(@"^\*\/(?<d>(60)|([1-5][0-9])|[1-9])$");

		public CronMinutes(String expr)
		{
			if ("*" == expr)
			{
				runMinutes = null;
				denominator = null;
			}
			else if (minutesRegex.IsMatch(expr))
			{
				//todo: check for duplicate values
				runMinutes = Regex.Split(expr, ",")
					.Select(e => UInt16.Parse(e))
					.ToArray();
				denominator = null;
			}
			else if (denominatedMinutesRegex.IsMatch(expr))
			{
				runMinutes = null;
				String dString = denominatedMinutesRegex.Match(expr).Groups["d"].Value;
				denominator = UInt16.Parse(dString);
			}
			else
			{
				throw new ArgumentException(expr + " is not a valid minutes term");
			}
		}

		public override String ToString()
		{
			if (null == runMinutes && null == denominator)
			{
				return "*";
			}
			else if (runMinutes != null && denominator == null)
			{
				StringBuilder exprBuilder = new StringBuilder(String.Empty);
				exprBuilder.Append(runMinutes[0]);
				if (runMinutes.Count() > 1)
				{
					for (int i = 1; i < runMinutes.Count(); i++)
					{
						exprBuilder.AppendFormat(",{0}", runMinutes[i]);
					}
				}
				return exprBuilder.ToString();
			}
			else if (runMinutes == null && denominator != null)
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
	/// A class encapsulating the hours term of a CronExpression
	/// </summary>
	public class CronHours
	{
		private UInt16[] runHours;
		private UInt16? denominator;

		public Regex hourRegex = new Regex(@"^((2[0-3])|(1[\d])|([\d]))(,((2[0-3])|(1[\d])|([\d]))){0,23}$");
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
	/// A class encapsulating the days term of a CronExpression
	/// </summary>
	public class CronDays
	{
		private UInt16[] runDays;
		private UInt16? denominator;

		public Regex daysRegex = new Regex(@"^((3[01])|([12][\d])|[\d])(,((3[01])|([12][\d])|[\d])){0,30}$");
		public Regex denominatedDaysRegex = new Regex(@"^\*\/((3[01])|([12][\d])|[1-9])$");

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
	/// A class encapsulating the months term of a CronExpression
	/// </summary>
	public class CronMonths
	{
		public CronMonths(String expr)
		{

		}
	}

	/// <summary>
	/// A class encapsulating the day of week term of the CronExpression
	/// </summary>
	public class CronWeekDays
	{
		public CronWeekDays(String expr)
		{

		}
	}
}