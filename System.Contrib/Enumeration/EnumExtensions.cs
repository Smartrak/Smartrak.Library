using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Contrib.Enumeration
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the individual flags set on this [Flags] enum value
		/// </summary>
		public static IEnumerable<Enum> GetFlags(this Enum enumValue)
		{
			return EnumUtil.GetValues(enumValue.GetType()).Where(enumValue.HasFlag);
		}

		/// <summary>
		/// Gets the attributes set on this [Flags] enum value
		/// </summary>
		public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum enumValue) where  TAttribute : Attribute
		{
			var type = enumValue.GetType();
			var attrType = typeof(TAttribute);

			return enumValue
				.GetFlags()
				.SelectMany(f => type.GetMember(f.ToString()))
				.SelectMany(m => m.GetCustomAttributes(attrType, true))
				.Cast<TAttribute>();
		}

		/// <summary>
		/// Gets the value of the [Description] attribute attached to this enum value.
		/// If this is a [Flags] enum, multiple descriptions will be comma separated.
		/// </summary>
		public static string GetDescription(this Enum enumValue)
		{
			return string.Join(", ", enumValue.GetAttributes<DescriptionAttribute>().Select(d => d.Description));
		}

		/// <summary>
		/// Returns true if this enum value has the [Obsolete] attribute.
		/// </summary>
		public static bool IsObsolete(this Enum enumValue)
		{
			return enumValue.GetAttributes<ObsoleteAttribute>().Any();
		}
	}
}
