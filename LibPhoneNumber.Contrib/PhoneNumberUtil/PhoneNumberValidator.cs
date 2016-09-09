using PhoneNumbers;

namespace LibPhoneNumber.Contrib.PhoneNumberUtil
{
	public static class PhoneNumberValidator
	{
		public static bool IsValid(string numberString, string[] regionCodes)
		{
			PhoneNumber phoneNumber;
			return PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetValidNumber(numberString, regionCodes, out phoneNumber);
		}

		/// <returns>Null for invalid input</returns>
		public static PhoneNumber GetValidPhoneNumber(string numberString, string[] regionCodes)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetValidNumber(numberString, regionCodes);
		}

		public static bool IsValidMobileNumber(string numberString, string[] regionCodes)
		{
			PhoneNumber phoneNumber;
			return PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetValidMobileNumber(numberString, regionCodes, out phoneNumber);
		}

		/// <returns>Null for invalid input</returns>
		public static PhoneNumber GetValidMobileNumber(string numberString, string[] regionCodes)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetValidMobileNumber(numberString, regionCodes);
		}
	}
}
