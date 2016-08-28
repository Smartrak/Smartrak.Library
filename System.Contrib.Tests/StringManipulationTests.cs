using System.Contrib.StringManipulation;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	class StringManipulationTests
	{
		[TestCase("abc<img src='lol.png'/>", "abc")]
		[TestCase("abc <span style='font-weight: bold'>troll</span>", "abc troll")]
		public void RemoveHtmlTags(string input, string expectedResult)
		{
			var result = input.RemoveHtmlTags();
			Assert.AreEqual(expectedResult, result);
		}
	}
}
