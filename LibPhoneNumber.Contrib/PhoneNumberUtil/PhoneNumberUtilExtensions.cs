using PhoneNumbers;

namespace LibPhoneNumber.Contrib.PhoneNumberUtil
{
	public static class PhoneUtilExtensions
	{
		public static bool TryGetValidMobileNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes, out PhoneNumber phoneNumber)
		{
			phoneNumber = null;

			var number = phoneUtil.GetValidNumber(numberString, regionCodes);

			if (number == null)
				return false;

			return phoneUtil.GetNumberType(number) == PhoneNumberType.MOBILE;
		}

		/// <returns>Null for invalid input</returns>
		public static PhoneNumber GetValidMobileNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes)
		{
			var number = phoneUtil.GetValidNumber(numberString, regionCodes);

			if (number == null)
				return null;

			return phoneUtil.GetNumberType(number) == PhoneNumberType.MOBILE 
				? number
				: null;
		}

		public static bool TryGetValidNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes, out PhoneNumber phoneNumber)
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
		public static PhoneNumber GetValidNumber(this PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string[] regionCodes)
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

		private static PhoneNumber ParseFromString(PhoneNumbers.PhoneNumberUtil phoneUtil, string numberString, string regionCode)
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
	}
}
