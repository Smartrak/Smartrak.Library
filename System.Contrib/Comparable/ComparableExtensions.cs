namespace System.Contrib.Comparable
{
	public static class ComparableExtensions
	{
		public static T Min<T>(this T a, T b) where T : IComparable<T>
		{
			return a.CompareTo(b) > 0 ? b : a;
		}

		public static T Max<T>(this T a, T b) where T : IComparable<T>
		{
			return a.CompareTo(b) > 0 ? a : b;
		}
	}
}
