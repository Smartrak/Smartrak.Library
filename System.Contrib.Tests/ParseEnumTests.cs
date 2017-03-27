using NUnit.Framework;
using System.Contrib.Enumeration;

namespace System.Contrib.Tests
{
	[TestFixture]
	public class ParseEnumTests
	{
		[TestCase("qq", null)]
		[TestCase("AThing", TestEnum.AThing)]
		[TestCase("athing", TestEnum.AThing)]
		[TestCase("anotherthing", TestEnum.AnOtherThing)]
		public void EnumTest(string value, TestEnum? expected)
		{
			var result = value.ToEnum<TestEnum>();

			Assert.AreEqual(expected, result);
		}

		public enum TestEnum
		{
			AThing,
			AnOtherThing
		}
	}
}
