using DougKlassen.Revit.Cron.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
			String testExpr3 = "*/60";
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
			CronHours expr1, expr2, expr3, expr4;
			String testExpr1 = "*";
			String testExpr2 = "0,1,10,23";
			String testExpr3 = "*/24";
			String testExpr4 = "0-23";
			#endregion arrange

			#region act
			expr1 = new CronHours(testExpr1);
			expr2 = new CronHours(testExpr2);
			expr3 = new CronHours(testExpr3);
			expr4 = new CronHours(testExpr4);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr1, expr1.ToString());
			Assert.AreEqual(testExpr2, expr2.ToString());
			Assert.AreEqual(testExpr3, expr3.ToString());
			Assert.AreEqual(testExpr4, expr4.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseDaysExpression()
		{
			#region arrange
			CronDays expr1, expr2, expr3, expr4;
			String testExpr1 = "*";
			String testExpr2 = "1,10,31";
			String testExpr3 = "*/31";
			String testExpr4 = "1-31";
			#endregion arrange

			#region act
			expr1 = new CronDays(testExpr1);
			expr2 = new CronDays(testExpr2);
			expr3 = new CronDays(testExpr3);
			expr4 = new CronDays(testExpr4);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr1, expr1.ToString());
			Assert.AreEqual(testExpr2, expr2.ToString());
			Assert.AreEqual(testExpr3, expr3.ToString());
			Assert.AreEqual(testExpr4, expr4.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseMonthsExpression()
		{
			#region arrange
			CronMonths expr1, expr2, expr3, expr4;
			String testExpr1 = "*";
			String testExpr2 = "1,6,12";
			String testExpr3 = "*/12";
			String testExpr4 = "1-12";
			#endregion arrange

			#region act
			expr1 = new CronMonths(testExpr1);
			expr2 = new CronMonths(testExpr2);
			expr3 = new CronMonths(testExpr3);
			expr4 = new CronMonths(testExpr4);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr1, expr1.ToString());
			Assert.AreEqual(testExpr2, expr2.ToString());
			Assert.AreEqual(testExpr3, expr3.ToString());
			Assert.AreEqual(testExpr4, expr4.ToString());
			#endregion assert
		}

		[TestMethod]
		public void CanParseWeekdaysExpression()
		{
			#region arrange
			CronWeekDays expr1, expr2, expr3, expr4;
			String testExpr1 = "*";
			String testExpr2 = "0,1,6";
			String testExpr3 = "0-6";
			#endregion arrange

			#region act
			expr1 = new CronWeekDays(testExpr1);
			expr2 = new CronWeekDays(testExpr2);
			expr3 = new CronWeekDays(testExpr3);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr1, expr1.ToString());
			Assert.AreEqual(testExpr2, expr2.ToString());
			Assert.AreEqual(testExpr3, expr3.ToString());
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
			CronExpression exprMonthly = new CronExpression("0 0 1 * *");	//run at midnite on the first day of the month
			ICollection<DateTime> monthlyRunTimesEnum;
			#endregion arrange

			#region act
			monthlyRunTimesEnum = exprMonthly.GetAnnualRunTimes().ToList();
			#endregion act

			#region assert
			Assert.AreEqual(12, monthlyRunTimesEnum.Count);
			#endregion
		}
	}
}
