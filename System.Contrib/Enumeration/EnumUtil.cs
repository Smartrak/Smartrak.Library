using System.Collections.Generic;
using System.Linq;

namespace System.Contrib.Enumeration
{
	public static class EnumUtil
	{
		/// <summary>
		/// Gets the values definied on this enum type
		/// </summary>
		/// <remarks>
		/// Unlike Enum.GetValues(), this will return enum values that are defined twice with different names
		/// </remarks>
		public static IEnumerable<Enum> GetValues(Type type)
		{
			ThrowHelper.CheckTypeIsEnum(type);

			return Enum
				.GetNames(type)
				.Select(e => (Enum) Enum.Parse(type, e));
		}

		public static IEnumerable<EnumValue<T>> GetEnumValues<T>() where T : struct
		{
			return GetValues(typeof(T)).Select(e => new EnumValue<T>(e));
		}
	}
}
