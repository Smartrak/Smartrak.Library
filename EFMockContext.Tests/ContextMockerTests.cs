using System.Data.Entity;
using FakeDbSet;
using NUnit.Framework;

namespace Smartrak.EFMockContext.Tests
{
	public interface ITestContext
	{
		IDbSet<Entity> Entity1s { get; set; }
		IDbSet<Entity> Entity2s { get; set; }
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
			var sut = ContextMocker.CreateMockContext<ITestContext>();

			Assert.IsInstanceOf<InMemoryDbSet<Entity>>(sut.Entity1s);
			Assert.IsInstanceOf<InMemoryDbSet<Entity>>(sut.Entity2s);
			Assert.IsFalse(sut.Entity1s == sut.Entity2s);
		}

		[Test]
		public void SaveChangesDoesntFail()
		{
			var sut = ContextMocker.CreateMockContext<ITestContext>();

			var res = sut.SaveChanges();

			Assert.AreEqual(0, res);
		}
	}
}
