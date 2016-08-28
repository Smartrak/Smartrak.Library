namespace System.Contrib.Enumeration
{
	public static class ThrowHelper
	{
		public static void CheckTypeIsEnum(Type type)
		{
			if (!type.IsEnum)
				throw new ArgumentException(type + " is not an enum");
		}

		public static void CheckValueDefinedOnEnum<T>(T value) where T : struct
		{
			var type = typeof (T);

			CheckTypeIsEnum(type);

			if (!Enum.IsDefined(type, value))
				throw new ArgumentException(value + " is not defined on enum " + type);
		}
	}
}