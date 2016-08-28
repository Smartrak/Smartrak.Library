using System.Contrib.TimeSpan;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	public class TimeSpanExtensionsTests
	{
		[Test]
		public void TestAverageNonZero()
		{
			var t1 = System.TimeSpan.FromHours(3);
			var t2 = System.TimeSpan.FromHours(1);
			var t3 = System.TimeSpan.FromMinutes(30);

			var total = System.TimeSpan.Zero;
			total += t1;
			total += t2;
			total += t3;

			var average = total.Average(3);

			var expectedResult = new System.TimeSpan(0, 1, 30, 0);

			Assert.AreEqual(expectedResult, average);
		}

		[Test]
		public void TestAverageZero()
		{
			var t1 = System.TimeSpan.Zero;
			var t2 = System.TimeSpan.Zero;
			var t3 = System.TimeSpan.Zero;

			var total = System.TimeSpan.Zero;
			total += t1;
			total += t2;
			total += t3;

			var average = total.Average(3);

			Assert.AreEqual(System.TimeSpan.Zero, average);
		}
	}
}
