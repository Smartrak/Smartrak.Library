using System.Contrib.Enumeration;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class EnumUtilTests
	{
		[Test]
		public void GetValuesTest()
		{
			// Act
			var actualEnumValues = EnumUtil.GetValues(typeof (TestEnum));

			// Assert
			var expectedEnumValues = new[]
			{
				TestEnum.Value1,
				TestEnum.Value2,
				TestEnum.Value3A,
				TestEnum.Value3B
			};

			CollectionAssert.AreEqual(expectedEnumValues, actualEnumValues);
		}

		#region Test classes

		public enum TestEnum
		{
			Value1 = 1,
			Value2 = 2,
			Value3A = 3,
			Value3B = 3
		}

		#endregion
	}
}
