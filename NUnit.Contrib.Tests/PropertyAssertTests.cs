using System;
using NUnit.Framework;

namespace NUnit.Contrib.Tests
{
	[TestFixture]
	internal class PropertyAssertTests
	{
		[Test]
		public void NullEqualTest()
		{
			// Arrange
			TestClass expected = null;
			TestClass actual = null;

			// Act, Assert
			// ReSharper disable ExpressionIsAlwaysNull
			PropertyAssert.AreEqual(expected, actual);
			// ReSharper restore ExpressionIsAlwaysNull
		}

		[Test]
		public void NullNotEqualTest()
		{
			// Arrange
			TestClass expected = null;
			TestClass actual = new TestClass();

			// Act, Assert
			Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual));
		}

		[Test]
		public void IntNotEqualTest()
		{
			// Arrange
			const int expected = 1;
			const int actual = 2;

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual));

			var expectedExceptionMessage = AggregateLines(
				"  Values are not equal.",
				"  Expected: 1",
				"  But was:  2",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void IntArrayNotEqualTest()
		{
			// Arrange
			var expected = new[] { 1, 2 };
			var actual = new[] { 1, 3 };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at index [1].",
				"  Expected: 2",
				"  But was:  3",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void IntArrayDifferentLengthTest()
		{
			// Arrange
			var expected = new[] { 1, 2, 3 };
			var actual = new[] { 1, 2 };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"Values are not equal.",
				"  Expected length: 3",
				"  But was length:  2",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void IntPropNotEqualTest()
		{
			// Arrange
			var expected = new TestClass { IntProp = 1 };
			var actual = new TestClass { IntProp = 2 };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'IntProp'.",
				"  Expected: 1",
				"  But was:  2",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void StringPropNotEqualTest()
		{
			// Arrange
			var expected = new TestClass { StringProp = "123"};
			var actual = new TestClass { StringProp = null };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'StringProp'.",
				"  Expected: \"123\"",
				"  But was:  null",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void IntArrayPropNotEqualTest()
		{
			// Arrange
			var expected = new TestClass { IntArrayProp = new []{1, 2, 3}};
			var actual = new TestClass { IntArrayProp = new []{1, 2, 4}};

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'IntArrayProp' at index [2].",
				"  Expected: 3",
				"  But was:  4",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void IntArrayPropDifferentLengthTest()
		{
			// Arrange
			var expected = new TestClass { IntArrayProp = new[] { 1, 2, 3, 4 } };
			var actual = new TestClass { IntArrayProp = new[] { 1, 2, 4 } };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"Values differ at property 'IntArrayProp'.",
				"  Expected length: 4",
				"  But was length:  3",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void NestedFalseTest()
		{
			// Arrange
			var expected = new TestClass { NestedTestClassProp = new TestClass { IntProp = 1} };
			var actual = new TestClass { NestedTestClassProp = new TestClass { IntProp = 1 }};

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual));

			// With nested=false, we think the objects differ, but we dont know how
			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'NestedTestClassProp'.",
				"  Expected: <NUnit.Contrib.Tests.PropertyAssertTests+TestClass>",
				"  But was:  <NUnit.Contrib.Tests.PropertyAssertTests+TestClass>",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void NestedTruePassTest()
		{
			// Arrange
			var expected = new TestClass { NestedTestClassProp = new TestClass { IntProp = 1 } };
			var actual = new TestClass { NestedTestClassProp = new TestClass { IntProp = 1 } };

			// Act, Assert
			Assert.DoesNotThrow(() => PropertyAssert.AreEqual(expected, actual, true));
		}

		[Test]
		public void NestedTrueFailTest()
		{
			// Arrange
			var expected = new TestClass { NestedTestClassProp = new TestClass { IntProp = 1 } };
			var actual = new TestClass { NestedTestClassProp = new TestClass { IntProp = 2 } };

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'NestedTestClassProp.IntProp'.",
				"  Expected: 1",
				"  But was:  2",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void TestClassArrayWithNestedTruePassesTest()
		{
			// Arrange
			var expected = new TestClass { TestClassArrayProp = new []{new TestClass { IntProp = 1 }} };
			var actual = new TestClass { TestClassArrayProp = new []{ new TestClass { IntProp = 1 } }};

			// Act, Assert
			Assert.DoesNotThrow(() => PropertyAssert.AreEqual(expected, actual, true));
		}

		[Test]
		public void TestClassArrayWithNestedTrueFailsTest()
		{
			// Arrange
			var expected = new TestClass { TestClassArrayProp = new[] { new TestClass { IntProp = 1 }, new TestClass {StringProp = "A"} }};
			var actual = new TestClass { TestClassArrayProp = new []{ new TestClass { IntProp = 1 }, new TestClass {StringProp = "B"} }};

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'TestClassArrayProp[1].StringProp'.",
				"  String lengths are both 1. Strings differ at index 0.",
				"  Expected: \"A\"",
				"  But was:  \"B\"",
				"  -----------^",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void DoesNotTraverseSelfReferentialPropertiesTest()
		{
			// Arrange
			var expected = new TestClass();
			expected.NestedTestClassProp = expected;
			expected.IntProp = 1;

			var actual = new TestClass();
			actual.NestedTestClassProp = actual;
			actual.IntProp = 2;

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, true));

			// If the referential check was not present, the asserter would have navigated down to its maximum depth before eventually comparing the IntProp
			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'IntProp'.",
				"  Expected: 1",
				"  But was:  2",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void AssertFailsWithPropertyFilterTest()
		{
			// Arrange
			var filter = new PropertyFilter().AddFilter<TestClass>(t => new
			{
				t.StringProp
			});

			var expected = new TestClass
			{
				IntProp = 1,
				StringProp = "A",
			};

			var actual = new TestClass
			{
				IntProp = 2,
				StringProp = "B"
			};

			// Act, Assert
			var exception = Assert.Throws<AssertionException>(() => PropertyAssert.AreEqual(expected, actual, propertyFilter: filter));

			var expectedExceptionMessage = AggregateLines(
				"  Values differ at property 'StringProp'.",
				"  String lengths are both 1. Strings differ at index 0.",
				"  Expected: \"A\"",
				"  But was:  \"B\"",
				"  -----------^",
				"");

			Assert.AreEqual(expectedExceptionMessage, exception.Message);
		}

		[Test]
		public void AssertPassesWithPropertyFilterTest()
		{
			// Arrange
			var filter = new PropertyFilter().AddFilter<TestClass>(t => new
			{
				t.StringProp
			});

			var expected = new TestClass
			{
				IntProp = 1,
				StringProp = "A",
			};

			var actual = new TestClass
			{
				IntProp = 2, // Value is different, but the property is not in the filter
				StringProp = "A"
			};

			// Act, Assert
			Assert.DoesNotThrow(() => PropertyAssert.AreEqual(expected, actual, propertyFilter: filter));
		}

		private static string AggregateLines(params string[] lines)
		{
			return string.Join(Environment.NewLine, lines);
		}

		private class TestClass
		{
			public TestClass NestedTestClassProp { get; set; }
			public TestClass[] TestClassArrayProp { get; set; }

			public int IntProp { get; set; }
			public string StringProp { get; set; }

			public int[] IntArrayProp { get; set; }
		}
	}
} 
