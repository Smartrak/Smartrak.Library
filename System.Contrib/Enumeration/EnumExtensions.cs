using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System.Contrib.Enumeration
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Gets the individual flags set on this [Flags] enum value
		/// </summary>
		public static IEnumerable<Enum> GetFlags(this Enum enumValue)
		{
			var type = enumValue.GetType();

			if (type.GetCustomAttribute<FlagsAttribute>() == null)
				return Enumerable.Repeat(enumValue, 1);

			return EnumUtil.GetValues(type).Where(enumValue.HasFlag);
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
		/// Gets the string representation of the selected value of the given attribute on this enum.
		/// If this is a [Flags] enum, or there are multiple of the same attribute, the selected values will be comma separated.
		/// Returns null when the given attribute does not exist on this enum value.
		/// </summary>
		public static string GetAttributeValues<TAttribute>(this Enum enumValue, Func<TAttribute, object> valueSelector) where TAttribute : Attribute
		{
			var attr = enumValue.GetAttributes<TAttribute>().ToArray();
			if (!attr.Any())
				return null;

			return string.Join(", ", attr.Select(valueSelector.Invoke));
		}

		/// <summary>
		/// Gets the value of the [Description] attribute attached to this enum value.
		/// </summary>
		public static string GetDescription(this Enum enumValue)
		{
			return enumValue.GetAttributeValues<DescriptionAttribute>(a => a.Description);
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
