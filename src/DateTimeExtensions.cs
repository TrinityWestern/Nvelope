using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nvelope
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Formats the date in the form used by the Astra database
        /// </summary>
        [Obsolete("Astra specific")]
        public static int ToJulianDate(this DateTime date)
        {
            return date.Year * 1000 + date.DayOfYear;
        }

        /// <summary>
        /// Gets the time as a 24hour-based integer, e.g. 11:30 PM -> 2330
        /// </summary>
        [Obsolete("This code is bad because it takes a string representation "
            + "of a date and encodes it as an integer. Just use "
            + "DateTime.ToString(). This is ASTRA specific.")]
        public static int ToIntTime(this DateTime date)
        {
            return date.Hour * 100 + date.Minute;
        }

        /// <summary>
        /// Gets the day of the week as a letter U/M/T/W/R/F/S
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [Obsolete("These UMTWRFS codes are ASTRA specific and do not belong "
            + "here")]
        public static string ToDayOfWeekLetter(this DateTime date)
        {
            string letters = "UMTWRFS";
            return letters[Convert.ToInt32(date.DayOfWeek)].ToString();
        }

        public static TimeSpan Age(this DateTime date)
        {
            return DateTime.Now - date;
        }

        public static string FuzzyName(this TimeSpan age)
        {
            int months = ((int)age.TotalDays) / 30;

            if (months > 0)
                return months + " " + (months > 1 ? "Months" : "Month");
            else if (age.TotalDays > 2)
                return (int)age.TotalDays + " " + (age.TotalDays > 1 ? "Days" : "Day");
            else if (age.TotalHours > 2)
                return (int)age.TotalHours + " " + (age.TotalHours > 1 ? "Hrs" : "Hour");
            else if (age.TotalMinutes > 1)
                return (int)age.TotalMinutes + " " + (age.TotalMinutes > 1 ? "Mins" : "Min");
            else
                return (int)age.TotalSeconds + " " + (age.TotalSeconds > 1 ? "Secs" : "Sec");
        }

        public static DateTime ToUtc(this DateTime date)
        {
            return
            new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the ISO formatted date
        /// </summary>
        public static string ToIsoDate(this DateTime source)
        {
            return source.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Returns the ISO formatted date (or a blank string)
        /// </summary>
        public static string ToIsoDate(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToIsoDate() : "";
        }

        /// <summary>
        /// Returns the ISO formatted date, using a space instead of a T for the seperator
        /// </summary>
        public static string ToIsoDateTime(this DateTime source)
        {
            return source.ToString("s").Replace('T', ' ');
        }

        /// <summary>
        /// Returns the ISO formatted date (or a blank string), using a space instead of a T for the seperator
        /// </summary>
        public static string ToIsoDateTime(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToIsoDateTime() : "";
        }

        public static String ToPrettyTime(this DateTime date)
        {
            return date.ToString("h:mm tt").ToLower();
        }
        
        /// <summary>
        /// Checks if a given DateTime is on the same day as this one
        /// </summary>
        /// <param name="original">The first DateTime you are working with</param>
        /// <param name="date">The DateTime to compare it with</param>
        /// <returns>True if they are on the same day</returns>
        public static bool OnSameDayAs(this DateTime original, DateTime? date)
        {
            return original.Date == (date ?? DateTime.MinValue).Date;
        }

        public static bool SameMinuteAs(this DateTime me, DateTime? other)
        {
            DateTime d = other ?? DateTime.MinValue;
            return me.Year == d.Year && me.Month == d.Month &&
                me.Day == d.Day && me.Hour == d.Hour && me.Minute == d.Minute;
        }

        public static bool SameMonthAs(this DateTime me, DateTime other)
        {
            return me.Year == other.Year && me.Month == other.Month;
        }

        public static DateTime TruncSeconds(this DateTime me)
        {
            return new DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, 0);
        }

        public static DateTime StartOfMonth(this DateTime me)
        {
            return new DateTime(me.Year, me.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime me)
        {
            return new DateTime(me.Year, me.Month, me.DaysInMonth());
        }

        /// <summary>
        /// Determine if Date String is an actual date format = MM/DD/YYYY
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [Obsolete("Locale specific. This should be renamed to ValidAmericanDate")]
        public static bool ValidDate(string date)
        {
            var format = new Regex(@"(\d+)/(\d+)/(\d+)");
            return format.IsMatch(date);
        }

        /// <summary>
        /// Returns a "friendly" formatted date
        /// </summary>
        public static string ToFriendlyDate(this DateTime date)
        {
            DateTime now = DateTime.Now;
            bool showTime = true;
            string day = date.ToString("s").Substring(0, 10);
            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                showTime = false;

            if (date.Date == now.Date)
                day = "Today";

            else if (date.Date == now.AddDays(-1).Date)
                day = "Yesterday";

            else if (date.Date == now.AddDays(1).Date)
                day = "Tomorrow";

            else if (date < now) //past
            {
                showTime = false;
                if (date > now.AddDays(-5))
                    day = date.DayOfWeek.ToString();

                else if (date.Date.Year == now.Year)
                    day = date.ToString("m");
            }

            else if (date > now) //future
            {
                if (date < now.AddDays(6))
                    day = "This " + date.DayOfWeek;
                else
                    showTime = false;
            }

            return day + (showTime ? " @ " + date.ToShortTimeString() : "");
        }

        /// <summary>
        /// Returns a "friendly" formatted date (or a blank string)
        /// </summary>
        public static string ToFriendlyDate(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToFriendlyDate() : "";
        }

        /// <summary>
        /// Is this datetime from the current calendar day?
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsToday(this DateTime source)
        {
            return OnSameDayAs(source, DateTime.Now);
        }

        public static int MonthsDifference(this DateTime start, DateTime end)
        {
            int yearDiff = end.Year - start.Year;

            int monthDiff = end.Month - start.Month;
            // If the end day is less than start day, then less
            // than a full month has passed
            if (end.Day < start.Day) {
                monthDiff -= 1;
            }
            return yearDiff * 12 + monthDiff;
        }

        public static int DaysInYear(this DateTime dt)
        {
            if (System.DateTime.IsLeapYear(dt.Year))
                return 366;
            else
                return 365;
        }

        public static int DaysInMonth(this DateTime dt)
        {
            return System.DateTime.DaysInMonth(dt.Year, dt.Month);
        }

        public static bool IsEndOfMonth(this DateTime dt)
        {
            return dt.AddDays(1).Month != dt.Month;
        }

    }
}
