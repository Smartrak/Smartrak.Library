using System.Linq;
using NUnit.Framework;

namespace NUnit.Contrib.Tests
{
	[TestFixture]
	internal class PropertyFilterTests
	{
		[Test]
		public void AddSingleImmediatePropTest()
		{
			// Arrange
			var mapTypePropertyMap = new PropertyFilter()
				.AddFilter<ClassA>(t => t.IntProp);

			// Act
			var actualProps = mapTypePropertyMap.ToArray();

			// Assert
			Assert.AreEqual(1, actualProps.Length);
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "IntProp"));
		}

		[Test]
		public void AddMultipleImmediatePropsTest()
		{
			// Arrange
			var mapTypePropertyMap = new PropertyFilter()
				.AddFilter<ClassA>(t => new
				{
					t.IntProp,
					t.DoubleProp
				});

			// Act
			var actualProps = mapTypePropertyMap.ToArray();

			// Assert
			Assert.AreEqual(2, actualProps.Length);
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "IntProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "DoubleProp"));
		}

		[Test]
		public void AddSingleNestedPropTest()
		{
			// Arrange
			var mapTypePropertyMap = new PropertyFilter()
				.AddFilter<ClassA>(t => t.BProp.CProp.FloatProp);

			// Act
			var actualProps = mapTypePropertyMap.ToArray();

			// Assert
			Assert.AreEqual(3, actualProps.Length);
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "BProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "CProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "FloatProp"));
		}

		[Test]
		public void AddMultipleNestedPropsTest()
		{
			// Arrange
			var mapTypePropertyMap = new PropertyFilter()
				.AddFilter<ClassA>(t => new
				{
					t.IntProp,
					t.BProp.CProp.FloatProp
				});

			// Act
			var actualProps = mapTypePropertyMap.ToArray();

			// Assert
			Assert.AreEqual(4, actualProps.Length);
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "IntProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "BProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "CProp"));
			Assert.AreEqual(1, actualProps.Count(p => p.Name == "FloatProp"));
		}

		private class ClassA
		{
			public int IntProp { get; set; }
			public double DoubleProp { get; set; }
			public string StringProp { get; set; }
			public ClassB BProp { get; set; }
		}

		private class ClassB
		{
			public ClassC CProp { get; set; }
		}

		private class ClassC
		{
			public float FloatProp { get; set; }
		}
	}
}
