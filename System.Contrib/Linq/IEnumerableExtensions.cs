using System.Collections.Generic;
using System.Linq;

namespace System.Contrib.Linq
{
	// ReSharper disable InconsistentNaming
	public static class IEnumerableExtensions
	// ReSharper restore InconsistentNaming
	{
		public static TAggregate Aggregate<TAggregate, TSource>(this IEnumerable<TSource> source, TAggregate seed, Action<TAggregate, TSource> accumulator)
		{
			foreach (var item in source)
				accumulator.Invoke(seed, item);

			return seed;
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		/// <summary>
		/// Checks that this collection has the same contents as another, ignoring order. Duplicates existing in one must exist in the other to be equivalent
		/// </summary>
		public static bool EquivalentTo<T>(this IEnumerable<T> source, IEnumerable<T> other)
		{
			// If expected implements ICollection, use it -- otherwise convert to array.
			var sourceCopy = source as ICollection<T> ?? source.ToArray();
			var otherCopy = new List<T>(other);

			return sourceCopy.Count == otherCopy.Count && sourceCopy.All(otherCopy.Remove) && otherCopy.Count == 0;
		}
	}
}
