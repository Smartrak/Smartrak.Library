namespace LibPhoneNumber.Contrib.PhoneNumberUtil
{
	/// <summary>
	/// Methods to validate a phonenumber
	/// </summary>
	public static class PhoneNumberValidator
	{
		/// <summary>
		/// Checks if phonenumber is valid or not
		/// </summary>
		/// <param name="numberString"></param>
		/// <param name="regionCodes"></param>
		/// <returns></returns>
		public static bool IsValid(string numberString, string[] regionCodes)
		{
			PhoneNumbers.PhoneNumber phoneNumber;
			return PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetValidNumber(numberString, regionCodes, out phoneNumber);
		}

		/// <summary>
		/// Returns the PhoneNumbers.PhoneNumber object if it is a valid number
		/// </summary>
		/// <param name="numberString">The number to validate against</param>
		/// <param name="regionCodes">The regions to check</param>
		/// <returns>Null if phonenumber was invalid</returns>
		public static PhoneNumbers.PhoneNumber GetValidPhoneNumber(string numberString, string[] regionCodes)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetValidNumber(numberString, regionCodes);
		}

		/// <summary>
		/// Check if phonenumber passed in is a valid mobile number in the specified regions
		/// </summary>
		/// <param name="numberString">Phonenumber</param>
		/// <param name="regionCodes">The region code for global networks</param>
		/// <returns>True if successful; else false</returns>
		public static bool IsValidMobileNumber(string numberString, string[] regionCodes)
		{
			PhoneNumbers.PhoneNumber phoneNumber;
			return PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetValidMobileNumber(numberString, regionCodes, out phoneNumber);
		}

		/// <summary>
		/// Returns the PhoneNumbers.PhoneNumber object of the phonenumber
		/// </summary>
		/// <param name="numberString">Phonenumber</param>
		/// <param name="regionCodes">The region code for global networks</param>
		/// <returns>Null if it was invalid phonenumber</returns>
		public static PhoneNumbers.PhoneNumber GetValidMobileNumber(string numberString, string[] regionCodes)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetValidMobileNumber(numberString, regionCodes);
		}

		/// <summary>
		/// Gets the phonenumber formatted to E164
		/// </summary>
		/// <param name="numberString">Phonenumber to validate and format</param>
		/// <param name="countryCodes">The country codes used in global networks</param>
		/// <returns></returns>
		public static string GetFormattedPhoneNumber(string numberString, string[] countryCodes)
		{
			string formattedPhoneNumber;
			PhoneNumbers.PhoneNumberUtil.GetInstance().TryGetFormattedPhoneNumber(numberString, countryCodes, out formattedPhoneNumber);
			return formattedPhoneNumber;
		}
	}
}
