using System.Data.Entity;
using NUnit.Framework;

namespace FakeDbSet.ContextGenerator.Tests
{
	public interface ITestContext
	{
		IDbSet<Entity> Entity1s { get; }
		IDbSet<Entity> Entity2s { get; }
		int SaveChanges();
	}

	public class Entity
	{
		public int Id { get; set; }
	}

	[TestFixture]
	public class ContextMockerTests
	{
		[Test]
		public void TestPropertiesCreated()
		{
			var sut = ContextGenerator.Generate<ITestContext>();

			Assert.IsInstanceOf<InMemoryDbSet<Entity>>(sut.Entity1s);
			Assert.IsInstanceOf<InMemoryDbSet<Entity>>(sut.Entity2s);
			Assert.IsFalse(sut.Entity1s == sut.Entity2s);
			Assert.IsTrue(sut.Entity1s == sut.Entity1s);
		}

		[Test]
		public void SaveChangesDoesntFail()
		{
			var sut = ContextGenerator.Generate<ITestContext>();

			var res = sut.SaveChanges();

			Assert.AreEqual(0, res);
		}
	}
}
