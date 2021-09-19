using System;

// ReSharper disable UnusedMember.Global

namespace MyFinance.Utilities
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			var diff = dt.DayOfWeek - startOfWeek;
			if (diff < 0)
			{
				diff += 7;
			}

			return dt.AddDays(-1 * diff).Date;
		}

		public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			var dateTime = dt.StartOfWeek(startOfWeek);
			dateTime = dateTime.AddDays(7);
			return dateTime;
		}
	}
}
