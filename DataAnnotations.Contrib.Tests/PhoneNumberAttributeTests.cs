using NUnit.Framework;

namespace DataAnnotations.Contrib.Tests
{
	[TestFixture]
	public class PhoneNumberAttributeTests
	{
		[Test]
		public void PhoneNumbersNoCountry()
		{
			var sut = new PhoneNumberAttribute();

			Assert.IsTrue(sut.IsValid("")); //empty
			Assert.IsTrue(sut.IsValid((string)null)); //null
			Assert.IsTrue(sut.IsValid(1)); //not string
			Assert.IsTrue(sut.IsValid("+64 21 000 0000")); //valid number
			Assert.IsTrue(sut.IsValid("+64210000000")); //valid number no spaces
			Assert.IsTrue(sut.IsValid("+64 21-00 000 00")); //valid number weird spaces and dashes


			Assert.IsFalse(sut.IsValid("021 000 0000")); //invalid number as no country code specified
			Assert.IsFalse(sut.IsValid("+64 21 000 00")); //invalid number as too short
			Assert.IsFalse(sut.IsValid("+64 21 000 000000")); //invalid number as too long
		}
		[Test]
		public void PhoneNumbersCountry()
		{
			var sut = new PhoneNumberAttribute("NZ");

			Assert.IsTrue(sut.IsValid("")); //empty
			Assert.IsTrue(sut.IsValid((string)null)); //null
			Assert.IsTrue(sut.IsValid(1)); //not string
			Assert.IsTrue(sut.IsValid("+64 21 000 0000")); //valid international number
			Assert.IsTrue(sut.IsValid("+64210000000")); //valid number no spaces
			Assert.IsTrue(sut.IsValid("021 000 0000")); //valid number as country code specified
			Assert.IsTrue(sut.IsValid("021-00 000 00")); //valid number weird spaces and dashes

			Assert.IsFalse(sut.IsValid("021 000 00")); //invalid number as too short
			Assert.IsFalse(sut.IsValid("021 000 000000")); //invalid number as too long
		}
	}
}
