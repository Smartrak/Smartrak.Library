using System.Collections.Generic;
using System.Contrib.DateTime;
using System.Linq;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class DateTimeExtensionsTests
	{
		[Test]
		[TestCaseSource(nameof(EndOfMonthTestCaseSource))]
		public void EndOfMonthTest(System.DateTime date, System.DateTime expectedEnfOfMonth)
		{
			// Act
			var actualEndOfMonth = date.EndOfMonth();

			// Assert
			Assert.AreEqual(expectedEnfOfMonth, actualEndOfMonth);
		}

		[Test]
		public void RangeOverDaysForZeroDaysReturnsNothingTest()
		{
			var date = new System.DateTime(2000, 01, 01);

			// Act
			var actualDates = date.RangeOverDays(0).ToArray();

			// Assert
			CollectionAssert.IsEmpty(actualDates);
		}

		[Test]
		public void RangeOverDaysForMultipleDaysReturnsInclusiveDaysTest()
		{
			var date = new System.DateTime(2000, 01, 01, 01, 01, 01);

			// Act
			var actualDates = date.RangeOverDays(3).ToArray();

			// Assert
			var expectedDates = new[]
			{
				new System.DateTime(2000, 01, 01, 01, 01, 01),
				new System.DateTime(2000, 01, 02, 01, 01, 01),
				new System.DateTime(2000, 01, 03, 01, 01, 01)
			};

			CollectionAssert.AreEqual(expectedDates, actualDates);
		}

		#region Test case sources

		public static IEnumerable<object[]> EndOfMonthTestCaseSource
		{
			get
			{
				yield return new object[] { new System.DateTime(2000, 01, 01, 01, 01, 01), new System.DateTime(2000, 01, 31, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2000, 01, 15, 01, 01, 01), new System.DateTime(2000, 01, 31, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2000, 01, 31, 01, 01, 01), new System.DateTime(2000, 01, 31, 01, 01, 01) };

				// February on normal year
				yield return new object[] { new System.DateTime(2015, 02, 01, 01, 01, 01), new System.DateTime(2015, 02, 28, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2015, 02, 15, 01, 01, 01), new System.DateTime(2015, 02, 28, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2015, 02, 28, 01, 01, 01), new System.DateTime(2015, 02, 28, 01, 01, 01) };

				// February on leap year
				yield return new object[] { new System.DateTime(2016, 02, 01, 01, 01, 01), new System.DateTime(2016, 02, 29, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2016, 02, 15, 01, 01, 01), new System.DateTime(2016, 02, 29, 01, 01, 01) };
				yield return new object[] { new System.DateTime(2016, 02, 29, 01, 01, 01), new System.DateTime(2016, 02, 29, 01, 01, 01) };
			}
		}

		#endregion
	}
}
