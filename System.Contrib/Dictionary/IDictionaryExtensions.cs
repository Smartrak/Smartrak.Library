using System.Collections.Generic;

namespace System.Contrib.Dictionary
{
	// ReSharper disable InconsistentNaming
	public static class IDictionaryExtensions
	// ReSharper restore InconsistentNaming
	{
		#region GetValueOrDefault()

		/// <summary>
		/// Gets the value at the given key, returing the given value if it doesnt exist
		/// </summary>
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValueProvider)
		{
			if (defaultValueProvider == null)
				throw new ArgumentNullException("defaultValueProvider");

			TValue value;
			return dict.TryGetValue(key, out value) ? value : defaultValueProvider.Invoke();
		}

		/// <summary>
		/// Gets the value at the given key, returing the given value if it doesnt exist
		/// </summary>
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
		{
			return dict.GetValueOrDefault(key, () => defaultValue);
		}

		/// <summary>
		/// Gets the value at the given key, returing the default of TValue if it doesnt exist
		/// </summary>
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			return dict.GetValueOrDefault(key, default(TValue));
		}

		#endregion

		#region GetValueOrAdd()

		/// <summary>
		/// Gets the value at the given key, adding the given value if it doesnt exist
		/// </summary>
		public static TValue GetValueOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValueProvider)
		{
			if (defaultValueProvider == null)
				throw new ArgumentNullException("defaultValueProvider");

			TValue value;
			if (dict.TryGetValue(key, out value))
				return value;

			value = defaultValueProvider.Invoke();
			dict.Add(key, value);
			return value;
		}

		/// <summary>
		/// Gets the value at the given key, adding the given value if it doesnt exist
		/// </summary>
		public static TValue GetValueOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
		{
			return dict.GetValueOrAdd(key, () => defaultValue);
		}

		/// <summary>
		/// Gets the value at the given key, adding the default of TValue if it doesnt exist
		/// </summary>
		public static TValue GetValueOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
		{
			return dict.GetValueOrAdd(key, default(TValue));
		}

		/// <summary>
		/// Gets the dictionary at the given key, adding one if it doesnt exist
		/// </summary>
		public static IDictionary<TInnerKey, TInnerValue> GetValueOrAdd<TKey, TInnerKey, TInnerValue>(this IDictionary<TKey, IDictionary<TInnerKey, TInnerValue>> dict, TKey key)
		{
			return dict.GetValueOrAdd(key, () => new Dictionary<TInnerKey, TInnerValue>());
		}

		/// <summary>
		/// Gets the list at the given key, adding one if it doesnt exist
		/// </summary>
		public static IList<TValue> GetValueOrAdd<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dict, TKey key)
		{
			return dict.GetValueOrAdd(key, () => new List<TValue>());
		}

		#endregion

		#region AddAtKey()

		/// <summary>
		/// Adds the given value to the end of the list at the given key.
		/// Creates the list if it does not exist.
		/// </summary>
		public static void AddAtKey<TKey, TValue>(this IDictionary<TKey, IList<TValue>> dict, TKey key, TValue value)
		{
			dict.GetValueOrAdd(key).Add(value);
		}

		#endregion
	}
}