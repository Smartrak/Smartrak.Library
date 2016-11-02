using System.Collections.Generic;
using System.Contrib.Linq;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	// ReSharper disable InconsistentNaming
	internal class IEnumerableExtensionsTests
	// ReSharper restore InconsistentNaming
	{
		#region Equivalent

		[Test]
		[TestCaseSource(nameof(GetEquivalentTestCases))]
		public void ListSourceListOtherEquivalent(IEnumerable<int> source, IEnumerable<int> other)
		{
			Assert.True(source.EquivalentTo(other));
		}

		[Test]
		[TestCaseSource(nameof(GetUnequivalentTestCases))]
		public void ListSourceListOtherUnequivalent(IEnumerable<int> source, IEnumerable<int> other)
		{
			Assert.False(source.EquivalentTo(other));
		}

		#endregion

		public static IEnumerable<object[]> GetEquivalentTestCases()
		{
			yield return new object[] { new int[0], new int[0] };
			yield return new object[] { new[] { 1, 2, 2, 3, 4, 5, 1 }, new[] { 1, 5, 4, 3, 2, 2, 1 } };
			yield return new object[] { new List<int> { 1, 2, 2, 3, 4, 5, 1 }, new List<int> { 1, 5, 4, 3, 2, 2, 1 } };
			yield return new object[] { new HashSet<int> { 1, 2, 2, 3, 4, 5, 1 }, new HashSet<int> { 1, 5, 4, 3, 2, 2, 1 } };
			yield return new object[] { new HashSet<int> { 1, 1, 1, 1 }, new HashSet<int> { 1 } };
		}

		public static IEnumerable<object[]> GetUnequivalentTestCases()
		{
			yield return new object[] { new[] { 1 }, new int[0] };
			yield return new object[] { new[] { 1, 2 }, new [] { 1 } };
			yield return new object[] { new List<int> { 1, 2, 2, 3 }, new List<int> { 1, 2, 3 } };
			yield return new object[] { new List<int> { 1, 2, 3 }, new List<int> { 1, 2, 2, 3 } };
			yield return new object[] { new HashSet<int> { 1, 2, 3, 1 }, new HashSet<int> { 1, 2, 3, 4 } };
		}
	}
}
