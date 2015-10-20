using System.Data.Entity;
using FakeDbSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

	[TestClass]
	public class ContextMockerTests
	{
		[TestMethod]
		public void TestPropertiesCreated()
		{
			var sut = ContextMocker.CreateMockContext<ITestContext>();


			Assert.IsInstanceOfType(sut.Entity1s, typeof(InMemoryDbSet<Entity>));
			Assert.IsInstanceOfType(sut.Entity2s, typeof(InMemoryDbSet<Entity>));
			Assert.AreNotEqual(sut.Entity1s, sut.Entity2s);
		}

		[TestMethod]
		public void SaveChangesDoesntFail()
		{
			var sut = ContextMocker.CreateMockContext<ITestContext>();

			var res = sut.SaveChanges();

			Assert.AreEqual(0, res);
		}
	}
}
