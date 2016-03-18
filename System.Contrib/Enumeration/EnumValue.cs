namespace System.Contrib.Enumeration
{
	/// <summary>
	/// Encapsulates the meta-data of an enum value
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EnumValue<T> where T : struct
	{
		public T Value { get; private set; }
		public Enum Enum { get; private set; }

		public int Id { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool Obsolete { get; private set; }

		private EnumValue(Enum enumValue, T value)
		{
			Enum = enumValue;
			Value = value;
			ThrowHelper.CheckTypeIsEnum(typeof(T));

			Id = Convert.ToInt32(Enum);
			Name = Enum.ToString();
			Description = Enum.GetDescription();
			Obsolete = Enum.IsObsolete();
		}

		public EnumValue(T value) : this ((Enum) (object) value, value)
		{
		}

		public EnumValue(Enum enumValue) : this(enumValue, (T) (object) enumValue)
		{
		}
	}

	public static class EnumValue
	{
		public static EnumValue<T> For<T>(T value) where T : struct
		{
			return new EnumValue<T>(value);
		}
	}
}