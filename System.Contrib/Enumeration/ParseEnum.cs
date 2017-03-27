namespace System.Contrib.Enumeration
{
	/// <summary>
	/// Helpers to parse an enum from a string
	/// </summary>
	public static class ParseEnum
	{
		/// <summary>
		/// Turns a string into an enum, adapted from http://stackoverflow.com/a/16104/1070291
		/// </summary>
		/// <typeparam name="T">The type of the enum</typeparam>
		/// <param name="value">a string representing the enum</param>
		/// <returns>The enum matching the string, or null if no match found</returns>
		public static T? ToEnum<T>(this string value) where T : struct, IConvertible
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			T result;
			return Enum.TryParse<T>(value, true, out result) ? result : (T?)null;
		}

		/// <summary>
		/// Turns a string into an enum, adapted from http://stackoverflow.com/a/16104/1070291
		/// </summary>
		/// <typeparam name="T">The type of the enum</typeparam>
		/// <param name="value">a string representing the enum</param>
		/// <param name="defaultValue">The enum to be returned if no match was found</param>
		/// <returns>The enum matching the string, or defaultValue if no match found</returns>
		public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IConvertible
		{
			return value.ToEnum<T>() ?? defaultValue;
		}
	}
}
