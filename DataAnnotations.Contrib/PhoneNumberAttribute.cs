using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace DataAnnotations.Contrib
{
	/// <summary>
	/// Validates phone numbers using libphonenumber, non-string fields and empty/null strings are considered valid. Use in conjunction with the [Required] if null/empty is not valid
	/// </summary>
	public class PhoneNumberAttribute : ValidationAttribute
	{
		private readonly string _countryCode;
		public PhoneNumberAttribute(string countryCode = "")
		{
			_countryCode = countryCode;
		}

		public override bool IsValid(object value)
		{
			var valueString = value as string;
			if (string.IsNullOrEmpty(valueString))
			{
				return true;
			}

			var util = PhoneNumberUtil.GetInstance();
			try
			{
				var number = util.Parse(valueString, _countryCode);
				return util.IsValidNumber(number);
			}
			catch (NumberParseException)
			{
				return false;
			}
		}
	}
}
