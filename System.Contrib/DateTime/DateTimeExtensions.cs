using System.Collections.Generic;

namespace System.Contrib.DateTime
{
	public static class DateTimeExtensions
	{
		public static System.DateTime EndOfMonth(this System.DateTime date)
		{
			return date.AddDays(System.DateTime.DaysInMonth(date.Year, date.Month) - date.Day);
		}

		public static IEnumerable<System.DateTime> Range(this System.DateTime startDate, System.DateTime endDateExclusive, TimeSpan interval)
		{
			for (var d = startDate; d <= endDateExclusive; d += interval)
				yield return d;
		}

		public static IEnumerable<System.DateTime> Range(this System.DateTime startDate, TimeSpan duration, TimeSpan interval)
		{
			return startDate.Range(startDate.Add(duration), interval);
		}

		public static IEnumerable<System.DateTime> RangeOverDays(this System.DateTime startDate, int days)
		{
			// Subtract 1 from the days, as it is counter intuitive to request the range over X days, but have X+1 DateTimes returned
			return startDate.Range(startDate.AddDays(days - 1), TimeSpan.FromDays(1));
		}
	}
}
