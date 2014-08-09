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
		public Regex minutesRegex = new Regex(@"^(([1-5][0-9])|([0-9]))(,(([1-5][0-9])|([0-9]))){0,59}$");	//todo: prevent duplicates
		/// <summary>
		/// match an expression denominated by a number in the range 1 to 60
		/// </summary>
		public Regex denominatedMinutesRegex = new Regex(@"^\*\/(?<d>(60)|([1-5][0-9])|([1-9]))$");

		public CronMinutes(String expr)
		{
			if (minutesRegex.IsMatch(expr))
			{
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
			else if (denominator == null)
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
			else if (runMinutes == null)
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

	}

	/// <summary>
	/// A class encapsulating the days term of a CronExpression
	/// </summary>
	public class CronDays
	{

	}

	/// <summary>
	/// A class encapsulating the months term of a CronExpression
	/// </summary>
	public class CronMonths
	{

	}

	/// <summary>
	/// A class encapsulating the day of week term of the CronExpression
	/// </summary>
	public class CronWeekDays
	{

	}
}