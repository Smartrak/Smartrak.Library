using NUnit.Framework;
using System.Contrib.Enumeration;

namespace System.Contrib.Tests
{
	[TestFixture]
	public class ParseEnumTests
	{
		[TestCase("qq", null, null)]
		[TestCase("AThing", null, TestEnum.AThing)]
		[TestCase("athing", null, TestEnum.AThing)]
		[TestCase("anotherthing", null, TestEnum.AnOtherThing)]
		[TestCase("qq", TestEnum.AThing, TestEnum.AThing)]
		[TestCase(null, TestEnum.AnOtherThing, TestEnum.AnOtherThing)]
		[TestCase("anotherthing", TestEnum.AThing, TestEnum.AnOtherThing)]
		public void EnumTest(string value, TestEnum? defaultValue, TestEnum? expected)
		{
			TestEnum? result;
			if (defaultValue.HasValue)
			{
				result = value.ToEnum(defaultValue.Value);
			}
			else
			{
				result = value.ToEnum<TestEnum>();
			}

			Assert.AreEqual(expected, result);
		}

		public enum TestEnum
		{
			AThing,
			AnOtherThing
		}
	}
}
