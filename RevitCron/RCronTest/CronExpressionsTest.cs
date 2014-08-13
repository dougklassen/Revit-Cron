using DougKlassen.Revit.Cron.Models;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DougKlassen.Revit.Cron.Test
{
	[TestClass]
	public class CronExpressionTests
	{
		[TestMethod]
		public void CanParseMinutesExpression()
		{
			#region arrange
			CronMinutes expr1, expr2, expr3, expr4;
			String testExpr1 = "*";
			String testExpr2 = "0,1,10,59";
			String testExpr3 = "*/10";
			String testExpr4 = "0-59";
			#endregion arrange

			#region act
			expr1 = new CronMinutes(testExpr1);
			expr2 = new CronMinutes(testExpr2);
			expr3 = new CronMinutes(testExpr3);
			expr4 = new CronMinutes(testExpr4);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr1, expr1.ToString());
			Assert.AreEqual(testExpr2, expr2.ToString());
			Assert.AreEqual(testExpr3, expr3.ToString());
			Assert.AreEqual(testExpr4, expr4.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseHoursExpression()
		{
			#region arrange
			CronHours expr;
			String testExpr = "0,1,10,23";
			#endregion arrange

			#region act
			expr = new CronHours(testExpr);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr, expr.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseDaysExpression()
		{
			#region arrange
			CronDays expr;
			String testExpr = "1,2,31";
			#endregion arrange

			#region act
			expr = new CronDays(testExpr);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr, expr.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseMonthsExpression()
		{
			#region arrange
			CronMonths expr;
			String testExpr = "1,2,12";
			#endregion arrange

			#region act
			expr = new CronMonths(testExpr);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr, expr.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseWeekdaysExpression()
		{
			#region arrange
			CronWeekDays expr;
			String testExpr = "0,1,6";
			#endregion arrange

			#region act
			expr = new CronWeekDays(testExpr);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr, expr.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseCronExpression()
		{
			#region arrange
			CronExpression[] expr;
			String[] testExpr = new String[] {
				"1 1 * * *",	//run at 0101 every month
				"* * * * *" };	//run every minute
			#endregion arrange

			#region act
			expr = Array.ConvertAll<String, CronExpression>(testExpr, s => new CronExpression(s));
			#endregion act

			#region assert
			for (int i = 0; i < expr.Length; i++)
			{
				Assert.AreEqual(testExpr[i], expr[i].ToString());
			}
			#endregion assert
		}

		[TestMethod]
		public void CanGetRunTimes()
		{
			#region arrange
			CronExpression exprMonthly = new CronExpression("0 0 * * *");	//run at midnite on the first day of the month
			ICollection<DateTime> monthlyRunTimes;
			#endregion arrange

			#region act
			monthlyRunTimes = (ICollection<DateTime>)exprMonthly.GetAnnualRunTimes();
			#endregion act

			#region assert
			Assert.AreEqual(12, monthlyRunTimes.Count);
			#endregion
		}
	}
}
