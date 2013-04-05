using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Nvelope
{
    public static class DateTimeExtensions
    {
        public static TimeSpan Age(this DateTime date)
        {
            return DateTime.Now - date;
        }

        public static string FuzzyName(this TimeSpan age)
        {
            int months = ((int)age.TotalDays) / 30;

            if(months > 0)
                return months + " " + (months > 1 ? "Months" : "Month");
            else if(age.TotalDays > 2)
                return (int)age.TotalDays + " " + (age.TotalDays > 1 ? "Days" : "Day");
            else if(age.TotalHours > 2)
                return (int)age.TotalHours + " " + (age.TotalHours > 1 ? "Hrs" : "Hour");
            else if(age.TotalMinutes > 1)
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
            return source.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the ISO formatted date (or a blank string)
        /// </summary>
        public static string ToIsoDate(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToIsoDate() : "";
        }

        /// <summary>
        /// Returns the .NET ToShortTimeString() value but works with null values returning a blank string
        /// </summary>
        public static string ToShortTimeString(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToShortTimeString() : "";
        }

        /// <summary>
        /// Returns the ISO formatted date, using a space instead of a T for the seperator
        /// </summary>
        public static string ToIsoDateTime(this DateTime source)
        {
            return source.ToString("s", CultureInfo.InvariantCulture).Replace('T', ' ');
        }

        /// <summary>
        /// Returns the ISO formatted date (or a blank string), using a space instead of a T for the seperator
        /// </summary>
        public static string ToIsoDateTime(this DateTime? source)
        {
            return source.HasValue ? source.Value.ToIsoDateTime() : "";
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
        public static string ToFriendlyDate(this DateTime date, IFormatProvider provider = null)
        {
            if(provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }

            DateTime now = DateTime.Now;
            bool showTime = true;
            string day = date.ToIsoDate();
            if(date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                showTime = false;

            if(date.Date == now.Date)
                day = "Today";

            else if(date.Date == now.AddDays(-1).Date)
                day = "Yesterday";

            else if(date.Date == now.AddDays(1).Date)
                day = "Tomorrow";

            else if(date < now) //past
            {
                showTime = false;
                if(date > now.AddDays(-5))
                    day = date.DayOfWeek.ToString();

                else if(date.Date.Year == now.Year)
                    day = date.ToString("m", provider);
            }

            else if(date > now) //future
            {
                if(date < now.AddDays(6))
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
        /// Converts to string, but handles null gracefully (converts to "")
        /// </summary>
        /// <param name="source">A nullable DateTime</param>
        /// <param name="format">A .NET DateTime formatting string</param>
        /// <returns></returns>
        public static string ToString(this DateTime? source, string format)
        {
            if(source == null)
                return string.Empty;
            else
                return source.Value.ToString(format);
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
            if(end.Day < start.Day)
            {
                monthDiff -= 1;
            }
            return yearDiff * 12 + monthDiff;
        }


        public static int DaysInYear(this DateTime dt)
        {
            if(System.DateTime.IsLeapYear(dt.Year))
                return 366;
            else
                return 365;
        }

        /// <summary>
        /// Returns the number of days in the month of this DateTime instance
        /// </summary>
        public static int DaysInMonth(this DateTime dt)
        {
            return System.DateTime.DaysInMonth(dt.Year, dt.Month);
        }

        /// <summary>
        /// Determines if this instance of a DateTime object is the last day in the month
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsEndOfMonth(this DateTime dt)
        {
            return dt.AddDays(1).Month != dt.Month;
        }

        /// <summary>
        /// Adds the specified number of months to a DateTime. Also sets the day of the month to match what you pass in, or
        /// the max day of the month, whichever is lesser. This function is useful when iterating through months and you
        /// initially start on the 31st and then in subsequent months get moved to 28 but then later want to return to the 31st.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="numMonths"></param>
        /// <param name="dayToMatch"></param>
        /// <returns></returns>
        public static DateTime AddMonthsMatchDay(this DateTime dt, int numMonths, int dayToMatch)
        {
            // Add the number of months to our original date
            var newDate = dt.AddMonths(numMonths);

            // Find which is less, the day to match or the number of days in the month
            var lesser = NumberExtensions.LesserOf(dayToMatch, newDate.DaysInMonth());

            // Add/Remove the number of days to match what was picked in the previous line as the lesser day
            return newDate.AddDays(lesser - newDate.Day);
        }


        /// <summary>
        /// This method check if given time is in within given time range
        /// </summary>
        /// <param name="time">Time to be valided</param>
        /// <param name="startTime">Start Time Range</param>
        /// <param name="endTime">End Time Range</param>
        /// <returns></returns>
        public static bool IsEnteredTimeWithTimeRange(TimeSpan enteredTime, TimeSpan startTime, TimeSpan endTime)
        {
            if(endTime == startTime)
            {
                return true;
            }
            else if(endTime < startTime)
            {
                if(enteredTime <= endTime || enteredTime >= startTime)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if(enteredTime >= startTime && enteredTime <= endTime)
                {
                    return true;
                }
                return false;
            }

        }

        /// <summary>
        /// This Method validates a give time.
        /// </summary>
        /// <param name="thetime">the time 21:00</param>
        /// <returns></returns>
        public static bool IsValidTime(string enteredTime)
        {
            enteredTime = Regex.Replace(enteredTime, "[A-Za-z]", "");
            Regex checkTime = new Regex(@"^(?:[01]?[0-9]|2[0-3]):[0-5][0-9]$");

            return checkTime.IsMatch(enteredTime);
        }
    }
}