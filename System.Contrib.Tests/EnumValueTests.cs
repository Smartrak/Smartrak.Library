using System.Contrib.Enumeration;
using NUnit.Framework;

namespace System.Contrib.Tests
{
	[TestFixture]
	internal class EnumValueTests
	{
		#pragma warning disable 612 // Disable obsolete warnings

		[Test]
		public void PopulatesPropertiesForEnumWithNoAttributesTest()
		{
			// Act
			var enumValue = EnumValue.For(TestEnum.Value4);

			// Assert
			Assert.AreEqual(TestEnum.Value4, enumValue.Value);
			Assert.AreEqual(Enum.Parse(typeof(TestEnum), "Value4"), enumValue.Enum);
			Assert.AreEqual(4, enumValue.Id);
			Assert.AreEqual("Value4", enumValue.Name);
			Assert.AreEqual(null, enumValue.Description);
			Assert.IsFalse(enumValue.Obsolete);
		}

		[Test]
		public void PopulatesPropertiesForSingleAttributedEnumTest()
		{
			// Act
			var enumValue = EnumValue.For(TestEnum.Value2);

			// Assert
			Assert.AreEqual(TestEnum.Value2, enumValue.Value);
			Assert.AreEqual(Enum.Parse(typeof(TestEnum), "Value2"), enumValue.Enum);
			Assert.AreEqual(2, enumValue.Id);
			Assert.AreEqual("Value2", enumValue.Name);
			Assert.AreEqual("Value2 Desc", enumValue.Description);
			Assert.IsTrue(enumValue.Obsolete);
		}

		[Test]
		public void PopulatesPropertiesForFlagsAttributedEnumTest()
		{
			// Act
			var enumValue = EnumValue.For(TestEnum.Value1 | TestEnum.Value2);

			// Assert
			Assert.AreEqual(TestEnum.Value1 | TestEnum.Value2, enumValue.Value);
			Assert.AreEqual(Enum.Parse(typeof(TestEnum), "Value1,Value2"), enumValue.Enum);
			Assert.AreEqual(3, enumValue.Id);
			Assert.AreEqual("Value1, Value2", enumValue.Name);
			Assert.AreEqual("Value1 Desc, Value2 Desc", enumValue.Description);
			Assert.IsTrue(enumValue.Obsolete);
		}

		[Test]
		public void CanUseEnumConstructorTest()
		{
			// Arrange
			var enumValue = new EnumValue<TestEnum>((Enum) Enum.Parse(typeof(TestEnum), "Value4"));

			// Assert
			Assert.AreEqual(TestEnum.Value4, enumValue.Value);
		}

		#region Test classes

		[Flags]
		public enum TestEnum
		{
			[ComponentModel.Description("Value1 Desc")]
			Value1 = 1,

			[Obsolete]
			[ComponentModel.Description("Value2 Desc")]
			Value2 = 2,

			Value4 = 4
		}

		#endregion

		#pragma warning restore 612
	}
}
