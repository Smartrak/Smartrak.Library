using System.Contrib.StringManipulation;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class StringExtensionsTests
	{
		[Test]
		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(" ", " ")]
		[TestCase("a", "A")]
		[TestCase("ab", "Ab")]
		[TestCase("Ab", "Ab")]
		[TestCase("AB", "AB")]
		[TestCase("abc", "Abc")]
		[TestCase("aB", "A B")]
		[TestCase("aBc", "A Bc")]
		[TestCase("abC", "Ab C")]
		[TestCase("Abc", "Abc")]
		[TestCase("ABC", "ABC")]
		[TestCase("abCd", "Ab Cd")]
		[TestCase("abCD", "Ab CD")]
		[TestCase("AbCd", "Ab Cd")]
		[TestCase("abCDe", "Ab C De")]
		[TestCase("NZIsAwesome", "NZ Is Awesome")]
		[TestCase("NzIsAwesome", "Nz Is Awesome")]
		[TestCase("IEnumerable", "I Enumerable")]
		[TestCase("IMyInterface", "I My Interface")]
		[TestCase("FooID", "Foo ID")]
		[TestCase("FooABar", "Foo A Bar")]
		[TestCase("FooIDBar", "Foo ID Bar")]
		[TestCase("FooBarBaz", "Foo Bar Baz")]
		[TestCase("fooBarBaz", "Foo Bar Baz")]
		[TestCase("foo BarBaz ID zip", "Foo Bar Baz ID zip")]
		public void ExpandCamelCaseTest(string input, string expectedOutput)
		{
			// Act
			var actualOutput = input.ExpandCamelCase();

			// Assert
			Assert.AreEqual(expectedOutput, actualOutput);
		}

		[Test]
		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(" ", " ")]
		[TestCase("a", "A")]
		[TestCase("ab", "Ab")]
		[TestCase("Ab", "Ab")]
		[TestCase("ABC", "ABC")]
		[TestCase("a b c", "A B C")]
		[TestCase("ab cd", "Ab Cd")]
		[TestCase("aB Cd", "Ab Cd")]
		[TestCase("aBc dEF", "Abc Def")]
		[TestCase("ABC def", "ABC Def")]
		public void ToTitleCaseTest(string input, string expectedOutput)
		{
			// Act
			var actualOutput = input.ToTitleCase();

			// Assert
			Assert.AreEqual(expectedOutput, actualOutput);
		}

		[Test]
		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(" ", "")]
		[TestCase("  ", "")]
		[TestCase("	", "")]
		[TestCase("	 ", "")]
		[TestCase(" a", "a")]
		[TestCase("a ", "a")]
		[TestCase(" a ", "a")]
		[TestCase("  a  ", "a")]
		[TestCase("a", "a")]
		[TestCase("ab", "ab")]
		[TestCase("a b", "a b")]
		[TestCase("a  b", "a b")]
		[TestCase("a	b", "a b")]
		[TestCase("a	 b", "a b")]
		[TestCase(" a	b	c ", "a b c")]
		public void TrimInternalTest(string input, string expectedOutput)
		{
			// Act
			var actualOutput = input.TrimInternal();

			// Assert
			Assert.AreEqual(expectedOutput, actualOutput);
		}
	}
}
