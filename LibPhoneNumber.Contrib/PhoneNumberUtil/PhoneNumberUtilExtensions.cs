using PhoneNumbers;

namespace LibPhoneNumber.Contrib.PhoneNumberUtil
{
	public static class PhoneUtilExtensions
	{
		public static bool TryGetValidMobileNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes, out PhoneNumbers.PhoneNumber phoneNumber)
		{
			phoneNumber = null;

			var number = phoneUtil.GetValidNumber(numberString, regionCodes);

			if (number == null)
				return false;

			return phoneUtil.GetNumberType(number) == PhoneNumberType.MOBILE;
		}

		/// <returns>Null for invalid input</returns>
		public static PhoneNumbers.PhoneNumber GetValidMobileNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes)
		{
			var number = phoneUtil.GetValidNumber(numberString, regionCodes);

			if (number == null)
				return null;

			return phoneUtil.GetNumberType(number) == PhoneNumberType.MOBILE 
				? number
				: null;
		}

		public static bool TryGetValidNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes, out PhoneNumbers.PhoneNumber phoneNumber)
		{
			phoneNumber = null;

			foreach (var regionCode in regionCodes)
			{
				phoneNumber = ParseFromString(phoneUtil, numberString, regionCode);

				if (phoneNumber == null)
					continue;

				if (phoneUtil.IsValidNumberForRegion(phoneNumber, regionCode))
					return true;
			}

			return false;
		}

		/// <returns>Null for invalid input</returns>
		public static PhoneNumbers.PhoneNumber GetValidNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes)
		{
			foreach (var regionCode in regionCodes)
			{
				var phoneNumber = ParseFromString(phoneUtil, numberString, regionCode);

				if (phoneNumber == null)
					continue;

				if (phoneUtil.IsValidNumberForRegion(phoneNumber, regionCode))
					return phoneNumber;
			}

			return null;
		}

		private static PhoneNumbers.PhoneNumber ParseFromString(PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string regionCode)
		{
			try
			{
				return phoneUtil.Parse(numberString, regionCode);
			}
			catch (NumberParseException)
			{
				return null;
			}
		}

		/// <summary>
		/// Try gets the phonenumber formatted in E164 if it was valid for the first country as specified in order of countries passed in
		/// </summary>
		/// <param name="phoneUtil">PhoneNumberUtil instance</param>
		/// <param name="numberString">The phonenumber to get </param>
		/// <param name="countryCodes">The countries to check for a valid phonenumber</param>
		/// <param name="formattedPhoneNumber">The phonenumber formatted in E164</param>
		/// <returns>True if successfully retrieves the formatted phonenumber</returns>
		public static bool TryGetFormattedPhoneNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] countryCodes, out string formattedPhoneNumber)
		{
			formattedPhoneNumber = null;

			foreach (var countryCode in countryCodes)
			{
				var phoneNumber = phoneUtil.Parse(numberString, countryCode);
				if (phoneUtil.IsValidNumber(phoneNumber))
				{
					formattedPhoneNumber = phoneUtil.Format(phoneNumber, PhoneNumberFormat.E164);
					return true;
				}
			}

			return false;
		}
	}
}
