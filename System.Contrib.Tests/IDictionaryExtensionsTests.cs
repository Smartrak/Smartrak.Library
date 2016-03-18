using System.Collections.Generic;
using System.Contrib.Dictionary;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	// ReSharper disable InconsistentNaming
	internal class IDictionaryExtensionsTests
	// ReSharper restore InconsistentNaming
	{
		#region GetValueOrDefault() Tests

		[Test]
		public void GetValueOrDefaultThrowsNullArgExceptionTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act, Assert
			Assert.Throws<ArgumentNullException>(() => dict.GetValueOrDefault(1, null));
		}

		[Test]
		public void GetValueOrDefaultForValueTypeTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act
			var actualExistsValue = dict.GetValueOrDefault(1);
			var actualDefaultValue = dict.GetValueOrDefault(2);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(0, actualDefaultValue); // Check default of int is used (0)

			Assert.AreEqual(1, dict.Count);
		}

		[Test]
		public void GetValueOrDefaultForReferenceTypeTest()
		{
			// Arrange
			var dict = new Dictionary<int, string> { { 1, "11" } };

			// Act
			var actualExistsValue = dict.GetValueOrDefault(1);
			var actualDefaultValue = dict.GetValueOrDefault(2);

			// Assert
			Assert.AreEqual("11", actualExistsValue);
			Assert.IsNull(actualDefaultValue); // Check default of string is used (null)

			Assert.AreEqual(1, dict.Count);
		}

		[Test]
		public void GetValueOrDefaultWithDefaultSuppliedTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act
			var actualExistsValue = dict.GetValueOrDefault(1, 111111);
			var actualDefaultValue = dict.GetValueOrDefault(2, 22);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(22, actualDefaultValue);

			Assert.AreEqual(1, dict.Count);
		}

		[Test]
		public void GetValueOrDefaultWithDefaultProviderSuppliedTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			bool providerAInvoked = false;
			bool providerBInvoked = false;

			Func<int> providerA = delegate
			{
				providerAInvoked = true;
				return 111111;
			};

			Func<int> providerB = delegate
			{
				providerBInvoked = true;
				return 22;
			};

			// Act
			var actualExistsValue = dict.GetValueOrDefault(1, providerA);
			var actualDefaultValue = dict.GetValueOrDefault(2, providerB);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(22, actualDefaultValue);

			Assert.IsFalse(providerAInvoked);
			Assert.IsTrue(providerBInvoked);

			Assert.AreEqual(1, dict.Count);
		}

		#endregion

		#region GetValueOrAdd() Tests

		[Test]
		public void GetValueOrAddThrowsNullArgExceptionTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act, Assert
			Assert.Throws<ArgumentNullException>(() => dict.GetValueOrAdd(1, null));
		}

		[Test]
		public void GetValueOrAddForValueTypeTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1);
			var actualDefaultValue = dict.GetValueOrAdd(2);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(0, actualDefaultValue); // Check default of int is used (0)

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.AreEqual(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		[Test]
		public void GetValueOrAddForReferenceTypeTest()
		{
			// Arrange
			var dict = new Dictionary<int, string> { { 1, "11" } };

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1);
			var actualDefaultValue = dict.GetValueOrAdd(2);

			// Assert
			Assert.AreEqual("11", actualExistsValue);
			Assert.IsNull(actualDefaultValue); // Check default of string is used (null)

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.IsNull(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		[Test]
		public void GetValueOrAddForListTypeTest()
		{
			// Arrange
			var list = new List<int>();
			var dict = new Dictionary<int, IList<int>> { { 1, list } };

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1);
			var actualDefaultValue = dict.GetValueOrAdd(2);

			// Assert
			Assert.AreSame(list, actualExistsValue);
			Assert.IsInstanceOf<List<int>>(actualDefaultValue); // Check new empty list is returned
			Assert.IsEmpty((List<int>) actualDefaultValue);

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.AreEqual(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		[Test]
		public void GetValueOrAddForDictionaryTypeTest()
		{
			// Arrange
			var innerDict = new Dictionary<int, string>();
			var dict = new Dictionary<int, IDictionary<int, string>> { { 1, innerDict } };

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1);
			var actualDefaultValue = dict.GetValueOrAdd(2);

			// Assert
			Assert.AreSame(innerDict, actualExistsValue);
			Assert.IsInstanceOf<Dictionary<int, string>>(actualDefaultValue); // Check new empty dictionary is returned
			Assert.IsEmpty((Dictionary<int, string>) actualDefaultValue);

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.AreEqual(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		[Test]
		public void GetValueOrAddWithDefaultSuppliedTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1, 111111);
			var actualDefaultValue = dict.GetValueOrAdd(2, 22);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(22, actualDefaultValue);

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.AreEqual(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		[Test]
		public void GetValueOrAddWithDefaultProviderSuppliedTest()
		{
			// Arrange
			var dict = new Dictionary<int, int> { { 1, 11 } };

			bool providerAInvoked = false;
			bool providerBInvoked = false;

			Func<int> providerA = delegate
			{
				providerAInvoked = true;
				return 111111;
			};

			Func<int> providerB = delegate
			{
				providerBInvoked = true;
				return 22;
			};

			// Act
			var actualExistsValue = dict.GetValueOrAdd(1, providerA);
			var actualDefaultValue = dict.GetValueOrAdd(2, providerB);

			// Assert
			Assert.AreEqual(11, actualExistsValue);
			Assert.AreEqual(22, actualDefaultValue);

			Assert.IsFalse(providerAInvoked);
			Assert.IsTrue(providerBInvoked);

			Assert.AreEqual(2, dict.Count);
			Assert.AreEqual(actualExistsValue, dict[1]);
			Assert.AreEqual(actualDefaultValue, dict[2]); // Check value was added to dict
		}

		#endregion

		#region AddAtKey() Tests

		[Test]
		public void AddAtKeyWhenKeyExistsTest()
		{
			// Arrange
			var dict = new Dictionary<int, IList<int>>
			{
				{1, new List<int> { 1, 2} }
			};

			// Act
			dict.AddAtKey(1, 3);

			// Assert
			Assert.AreEqual(1, dict.Keys.Count);
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, dict[1]);
		}

		[Test]
		public void AddAtKeyWhenKeyDoesNotExistTest()
		{
			// Arrange
			var dict = new Dictionary<int, IList<int>>
			{
				{1, new List<int> { 1, 2} }
			};

			// Act
			dict.AddAtKey(2, 3);

			// Assert
			Assert.AreEqual(2, dict.Keys.Count);
			CollectionAssert.AreEqual(new[] { 1, 2 }, dict[1]);
			CollectionAssert.AreEqual(new[] { 3 }, dict[2]);
		}

		#endregion
	}
}
