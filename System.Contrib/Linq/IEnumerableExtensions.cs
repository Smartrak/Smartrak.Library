using System.Collections.Generic;

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
	}
}
