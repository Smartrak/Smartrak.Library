namespace System.Contrib.TimeSpan
{
	public static class TimeSpanExtensions
	{
		public static System.TimeSpan Average(this System.TimeSpan t, int count)
		{
			return System.TimeSpan.FromTicks(t.Ticks / count);
		}
	}
}
