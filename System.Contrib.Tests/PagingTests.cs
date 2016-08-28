using System.Collections.Generic;
using NUnit.Framework;

namespace System.Linq.Contrib.Paging.Tests
{
	public class Entity
	{
		public int Id { get; set; }
	}

	[TestFixture]
	public class PagingTests
	{
		private IEnumerable<Entity> GetTestData(int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return new Entity { Id = i + 1 };
			}
		}

		[Test]
		public void TestFirstPage()
		{
			var result = GetTestData(100).AsQueryable().GetPageWithTotal(1, 10);
			Assert.AreEqual(10, result.Count());
			Assert.AreEqual(100, result.First().Count);
			for (int i = 0; i < 10; i++)
			{
				Assert.AreEqual(i + 1, result.ToArray()[i].Entity.Id);
			}
		}

		[Test]
		public void TestLastPage()
		{
			var result = GetTestData(99).AsQueryable().GetPageWithTotal(10, 10);
			Assert.AreEqual(9, result.Count());
			Assert.AreEqual(99, result.First().Count);
			for (int i = 0; i < 9; i++)
			{
				Assert.AreEqual(i + 91, result.ToArray()[i].Entity.Id);
			}
		}

		[Test]
		public void TestPageTooFar()
		{
			var result = GetTestData(99).AsQueryable().GetPageWithTotal(11, 10);
			Assert.AreEqual(0, result.Count());
		}
	}
}
