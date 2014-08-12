using DougKlassen.Revit.Cron.Models;
using System;
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
			CronMinutes expr;
			String testExpr = "0,1,10,59";
			#endregion arrange

			#region act
			expr = new CronMinutes(testExpr);
			#endregion act

			#region assert
			Assert.AreEqual(testExpr, expr.ToString());
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
	}
}
