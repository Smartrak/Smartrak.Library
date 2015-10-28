using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartrak.SystemExtensions.Parsing
{
	public static class ParseHelpers
	{
		public static int? ToInt(this string s)
		{
			int i;
			if (int.TryParse(s, out i)) return i;
			return null;
		}
		public static double? ToDouble(this string s)
		{
			double i;
			if (double.TryParse(s, out i)) return i;
			return null;
		}
		public static float? ToFloat(this string s)
		{
			float i;
			if (float.TryParse(s, out i)) return i;
			return null;
		}
		public static decimal? ToDecimal(this string s)
		{
			decimal i;
			if (decimal.TryParse(s, out i)) return i;
			return null;
		}
		public static short? ToShort(this string s)
		{
			short i;
			if (short.TryParse(s, out i)) return i;
			return null;
		}

		public static long? ToLong(this string s)
		{
			long i;
			if (long.TryParse(s, out i)) return i;
			return null;
		}

		public static DateTime? ToDateTime(this string s)
		{
			DateTime x;
			if (DateTime.TryParse(s, out x)) return x;
			return null;
		}

		public static DateTimeOffset? ToDateTimeOffset(this string s)
		{
			DateTimeOffset x;
			if (DateTimeOffset.TryParse(s, out x)) return x;
			return null;
		}
	}
}
