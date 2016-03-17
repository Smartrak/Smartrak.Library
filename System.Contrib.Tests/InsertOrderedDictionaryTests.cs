using System.Collections.Generic;
using System.Contrib.Dictionary;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class InsertOrderedDictionaryTests
	{
		// TODO: Test fail conditions (ie: Inserting at a key that already exists)
		// TODO: Test the Keys and Values collections are correctly updated upon add/remove/update

		#region Get tests

		[Test]
		public void KeysCollectionIsOrderedTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualKeys = dict.Keys;

			// Assert
			var expectedKeys = new[] {2, 3, 4, 5, 1};

			CollectionAssert.AreEqual(expectedKeys, actualKeys);
		}

		[Test]
		public void ValuesCollectionIsOrderedTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualValues = dict.Values;

			// Assert
			var expectedValues = new[] { "3", "4", "5", "1", "2" };

			CollectionAssert.AreEqual(expectedValues, actualValues);
		}

		[Test]
		public void EnumerationIsOrderedTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act, Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void GetValueUsingIndexerTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualValues = new[] { dict[2], dict[3], dict[4], dict[5], dict[1] };

			// Assert
			var expectedValues = new[] { "3", "4", "5", "1", "2" };

			CollectionAssert.AreEqual(expectedValues, actualValues);
		}

		[Test]
		public void TryGetValueWhenKeyExistsTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			string actualValue;
			var actualExists = dict.TryGetValue(4, out actualValue);

			// Assert
			Assert.IsTrue(actualExists);
			Assert.AreEqual("5", actualValue);
		}

		[Test]
		public void TryGetValueWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			string actualValue;
			var actualExists = dict.TryGetValue(6, out actualValue);

			// Assert
			Assert.IsFalse(actualExists);
			Assert.IsNull(actualValue);
		}

		#endregion

		#region Add tests

		[Test]
		public void AddKvpTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				// {5, "1"},
				// {1, "2"}
			};

			// Act
			dict.Add(new KeyValuePair<int, string>(5, "1"));
			dict.Add(new KeyValuePair<int, string>(1, "2"));

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void AddValueUsingIndexerTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				// {5, "1"},
				// {1, "2"}
			};

			// Act
			dict[5] = "1";
			dict[1] = "2";

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void PrependItemTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				// {2, "3"},
				// {3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			dict.Prepend(3, "4");
			dict.Prepend(2, "3");

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void UpdateExistingValueUsingIndexerTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "444"},
				{4, "5"},
				{5, "111"},
				{1, "2"}
			};

			// Act
			dict[3] = "4";
			dict[5] = "1";

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void InsertValueAfterMidKeyTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				// {4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			dict.InsertAfter(3, 4, "5");

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void InsertValueAfterLastKeyTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				// {1, "2"}
			};

			// Act
			dict.InsertAfter(5, 1, "2");

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void InsertValueBeforeMidKeyTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				// {4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			dict.InsertBefore(5, 4, "5");

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void InsertValueBeforeFirstKeyTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				// {2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			dict.InsertBefore(3, 2, "3");

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		#endregion

		#region Remove tests

		[Test]
		public void RemoveAtKeyTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualRemoved = dict.Remove(4);

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			Assert.IsTrue(actualRemoved);
			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void RemoveAtKeyFailsWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualRemoved = dict.Remove(6);

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			Assert.IsFalse(actualRemoved);
			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void RemoveKvpTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualRemoved = dict.Remove(new KeyValuePair<int, string>(4, "5"));

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			Assert.IsTrue(actualRemoved);
			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void RemoveKvpFailsWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualRemoved = dict.Remove(new KeyValuePair<int, string>(6, "0"));

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			Assert.IsFalse(actualRemoved);
			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		[Test]
		public void RemoveKvpFailsWhenValueNotEqualTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualRemoved = dict.Remove(new KeyValuePair<int, string>(4, "555"));

			// Assert
			var expectedKvp = new[]
			{
				new KeyValuePair<int, string>(2, "3"),
				new KeyValuePair<int, string>(3, "4"),
				new KeyValuePair<int, string>(4, "5"),
				new KeyValuePair<int, string>(5, "1"),
				new KeyValuePair<int, string>(1, "2")
			};

			Assert.IsFalse(actualRemoved);
			CollectionAssert.AreEqual(expectedKvp, dict);
		}

		#endregion

		#region Contains tests

		[Test]
		public void ContainsKeyReturnsTrueWhenKeyExistsTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualContains = dict.ContainsKey(4);

			// Assert
			Assert.IsTrue(actualContains);
		}

		[Test]
		public void ContainsKeyReturnsFalseWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualContains = dict.ContainsKey(6);

			// Assert
			Assert.IsFalse(actualContains);
		}

		[Test]
		public void ContainsReturnsTrueWhenKvpExistsTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualContains = dict.Contains(new KeyValuePair<int, string>(4, "5"));

			// Assert
			Assert.IsTrue(actualContains);
		}

		[Test]
		public void ContainsReturnsFalseWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualContains = dict.Contains(new KeyValuePair<int, string>(6, "5"));

			// Assert
			Assert.IsFalse(actualContains);
		}

		[Test]
		public void ContainsReturnsFalseWhenValueIsNotEqualTest()
		{
			// Arrange
			var dict = new InsertOrderedDictionary<int, string>
			{
				{2, "3"},
				{3, "4"},
				{4, "5"},
				{5, "1"},
				{1, "2"}
			};

			// Act
			var actualContains = dict.Contains(new KeyValuePair<int, string>(4, "555"));

			// Assert
			Assert.IsFalse(actualContains);
		}

		#endregion
	}
}