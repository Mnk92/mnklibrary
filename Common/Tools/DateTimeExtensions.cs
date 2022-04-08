using System;

namespace Mnk.Library.Common.Tools
{
    public static class DateTimeExtensions
    {
        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            var daysAhead = (DayOfWeek.Sunday - (int)date.DayOfWeek);
            return date.AddDays((int)daysAhead);
        }

        public static DateTime GetLastDayOfWeek(this DateTime date)
        {
            var daysAhead = DayOfWeek.Saturday - (int)date.DayOfWeek;
            return date.AddDays((int)daysAhead);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return date.AddDays(-date.Day+1);
        }

        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            var difference = DateTime.DaysInMonth(date.Year, date.Month) - date.Day;
            return date.AddDays(difference);
        }

        public static DateTime Normalize(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
    }
}
