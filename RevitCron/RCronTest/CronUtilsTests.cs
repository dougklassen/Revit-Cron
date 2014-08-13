using DougKlassen.Revit.Cron.Models;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DougKlassen.Revit.Cron.Test
{
	[TestClass]
	public class CronUtilsTests
	{
		[TestMethod]
		public void IsContiguousTest()
		{
			#region arrange
			Boolean nonContigEval;
			UInt16[] nonContig = new UInt16[] { 5, 6, 2, 1, 3, 4, 0, 8 };
			Boolean contigEval;
			UInt16[] contig = new UInt16[] { 5, 2, 1, 4, 6, 0, 3 };
			#endregion arrange

			#region act
			nonContigEval = nonContig.IsContiguous();
			contigEval = contig.IsContiguous();
			#endregion act

			#region assert
			Assert.IsFalse(nonContigEval);
			Assert.IsTrue(contigEval);
			#endregion
		}
	}
}
