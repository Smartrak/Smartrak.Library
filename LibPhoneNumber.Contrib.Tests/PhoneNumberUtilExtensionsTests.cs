using LibPhoneNumber.Contrib.PhoneNumberUtil;
using NUnit.Framework;

namespace LibPhoneNumber.Contrib.Tests
{
	[TestFixture]
	public class PhoneNumberUtilExtensionTests
	{
		[Test]
		[TestCase("+642102478765", true, "+642102478765")]
		[TestCase("642102478765", true, "+642102478765")]
		[TestCase("02102478765", true, "+642102478765")]
		[TestCase("+61467751171", true, "+61467751171")]
		[TestCase("61467751171", true, "+61467751171")]
		[TestCase("0467751171", true, "+61467751171")]
		public void TryGetFormattedPhoneNumberTest(string phoneNumber, bool tryGetResult, string outPhoneNumber)
		{
			var countryCodes = new [] { "NZ", "AU" };
			string formattedPhoneNumber;
			var x = PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetFormattedPhoneNumber(phoneNumber, countryCodes, out formattedPhoneNumber);

			Assert.AreEqual(tryGetResult, x);
			Assert.AreEqual(outPhoneNumber, formattedPhoneNumber);
		}
	}
}
