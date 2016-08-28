using System.Contrib.Enumeration;
using System.Linq;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class EnumExtensionsTests
	{
		#pragma warning disable 612 // Disable Obsolete warnings

		#region GetFlags() Tests

		[Test]
		public void GetFlagsReturnsSingleEnumTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.A;

			// Act
			var flags = enumValue.GetFlags().ToArray();

			// Assert
			Assert.AreEqual(1, flags.Length);
			Assert.AreEqual(TestFlagsEnum.A, flags[0]);
		}

		[Test]
		public void GetFlagsReturnsTwoEnumsTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.A | TestFlagsEnum.B;

			// Act
			var flags = enumValue.GetFlags().ToArray();

			// Assert
			Assert.AreEqual(2, flags.Length);
			Assert.AreEqual(TestFlagsEnum.A, flags[0]);
			Assert.AreEqual(TestFlagsEnum.B, flags[1]);
		}

		[Test]
		public void GetFlagsReturnsNonFlagEnumTest()
		{
			// Arrange
			const TestEnum enumValue = TestEnum.A1;

			// Act
			var flags = enumValue.GetFlags().ToArray();

			// Assert
			Assert.AreEqual(1, flags.Length); // Check TestEnum.A2 isnt returned, since its not a [Flags] enum
			Assert.AreEqual(TestEnum.A1, flags[0]);
		}

		#endregion

		#region GetDescription() tests

		[Test]
		public void GetDescriptionReturnsNullWhenNoDescriptionTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.C;

			// Act
			var description = enumValue.GetDescription();

			// Assert
			Assert.IsNull(description);
		}

		[Test]
		public void GetDescriptionReturnsSingleDescriptionTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.A;

			// Act
			var description = enumValue.GetDescription();

			// Assert
			Assert.AreEqual("DescA1", description);
		}

		[Test]
		public void GetDescriptionReturnsTwoDescriptionOnFlagsEnumTest()
		{
			// Arrange
			
			const TestFlagsEnum enumValue = TestFlagsEnum.A | TestFlagsEnum.B;
			

			// Act
			var description = enumValue.GetDescription();

			// Assert
			Assert.AreEqual("DescA1, DescB2", description);
		}

		#endregion

		#region IsObsolete() tests

		[Test]
		public void IsObsoleteReturnsFalseWhenAttributeNotPresentTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.A;

			// Act
			var isObsolete = enumValue.IsObsolete();

			// Assert
			Assert.IsFalse(isObsolete);
		}

		[Test]
		public void IsObsoleteReturnsTrueWhenAttributePresentTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.B;

			// Act
			var isObsolete = enumValue.IsObsolete();

			// Assert
			Assert.IsTrue(isObsolete);
		}

		[Test]
		public void IsObsoleteReturnsTrueWhenAttributePresentOnFlagsEnumTest()
		{
			// Arrange
			const TestFlagsEnum enumValue = TestFlagsEnum.A | TestFlagsEnum.B;

			// Act
			var isObsolete = enumValue.IsObsolete();

			// Assert
			Assert.IsTrue(isObsolete);
		}

		#endregion

		#region Test classes

		private enum TestEnum
		{
			A1 = 1,
			A2 = 1,
			B = 2
		}

		[Flags]
		private enum TestFlagsEnum
		{
			[ComponentModel.Description("DescA1")]
			A = 1,

			[Obsolete]
			[ComponentModel.Description("DescB2")]
			B = 2,

			[Obsolete]
			C = 4
		}

		#endregion

		#pragma warning restore 612
	}
}
