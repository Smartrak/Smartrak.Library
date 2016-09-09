using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneNumbers;

namespace LibPhoneNumber.Contrib.PhoneNumber
{
	public static class PhoneNumberExtensions
	{
		public static PhoneNumberType GetNumberType(this PhoneNumbers.PhoneNumber phoneNumber)
		{
			return PhoneNumbers.PhoneNumberUtil.GetInstance().GetNumberType(phoneNumber);
		}

		public static bool IsMobileNumber(this PhoneNumbers.PhoneNumber phoneNumber)
		{
			return  phoneNumber.GetNumberType() == PhoneNumberType.MOBILE;
		}
	}
}
