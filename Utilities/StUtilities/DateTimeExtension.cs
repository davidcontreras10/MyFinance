using System;

namespace StUtilities
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            DateTime dateTime = dt.StartOfWeek(startOfWeek);
            dateTime = dateTime.AddDays(7);
            return dateTime;
        }
    }
}
