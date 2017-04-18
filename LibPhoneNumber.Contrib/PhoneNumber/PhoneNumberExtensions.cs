using PhoneNumbers;

namespace LibPhoneNumber.Contrib.PhoneNumber
{
	/// <summary>
	/// PhoneNumber Extensions
	/// </summary>
	public static class PhoneNumberExtensions
	{
		/// <summary>
		/// Gets the PhoneNumberType of the PhoneNumber passed in
		/// </summary>
		/// <param name="phoneNumber">PhoneNumber</param>
		/// <returns>The phone number type</returns>
		public static PhoneNumberType GetNumberType(this PhoneNumbers.PhoneNumber phoneNumber)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetNumberType(phoneNumber);
		}

		/// <summary>
		/// Checks if the phonenumber object is mobile number
		/// </summary>
		/// <param name="phoneNumber">Phonenumber</param>
		/// <returns>True if phonenumber is a mobile number</returns>
		public static bool IsMobileNumber(this PhoneNumbers.PhoneNumber phoneNumber)
		{
			return  phoneNumber.GetNumberType() == PhoneNumberType.MOBILE;
		}
	}
}
