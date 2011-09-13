namespace Nvelope
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TimeSpanExtensions
    {
        public static TimeSpan ApproximateMonth = new TimeSpan(30, 0, 0, 0);
        public static TimeSpan ApproximateYear = new TimeSpan(365, 0, 0, 0);
        /// <summary>
        /// Rounding conversion of TimeSpan to Years
        /// </summary>
        public static int RoundToYears(this TimeSpan source)
        {
            return (source.TotalDays / 365).ConvertTo<decimal>().RoundTo().ConvertTo<int>();
        }

        /// <summary>
        /// <para>Creates an approximated timespan, rounded to the nearest
        /// significant unit.</para>
        /// <para>Anything under a second will become no time at all, anything
        /// under a mi</para>
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static TimeSpan Approximate(this TimeSpan interval)
        {
            if (interval.Days > 0)
            {
                int days;

                if (interval.Days >= ApproximateYear.Days)
                {
                    // TODO: be more accurate about 
                    var year = ApproximateYear.Days;
                    // take number of days and had half a year for rounding
                    days = interval.Days + (year / 2) + ((interval.Hours + 12) / 24);
                    // floor the values to the number of days in a year
                    days = (days / year) * year;
                }
                else if (interval.Days >= ApproximateMonth.Days)
                {
                    var month = ApproximateMonth.Days;
                    days = ((interval.Days + (month / 2)) / month) * month;
                    
                }
                else
                {
                    days = interval.Days + (interval.Hours / 24);
                }
                return new TimeSpan(days, 0, 0, 0);
            }
            if (interval.Hours > 0)
            {
                return new TimeSpan(interval.Hours + (interval.Minutes / 30), 0, 0);
            }
            if (interval.Minutes > 0)
            {
                return new TimeSpan(0, interval.Minutes + (interval.Seconds / 30), 0);
            }
            if (interval.Seconds > 0)
            {
                return new TimeSpan(0, 0, interval.Seconds + (interval.Milliseconds / 500));
            }
            return new TimeSpan(0);
        }

        public static string ToEnglish(this TimeSpan interval, bool spellOut)
        {
            // May a nice human readable string
            var parts = new List<string>();
            if (interval.Days > 0)
            {
                var years = interval.Days / ApproximateYear.Days;
                var days = (interval.Days % ApproximateYear.Days);
                var months = days / ApproximateMonth.Days;
                days = days % ApproximateMonth.Days;

                if (years > 0)
                {
                    parts.Add(English.Number("year", years, spellOut));
                }
                if (months > 0)
                {
                    parts.Add(English.Number("month", months, spellOut));
                }
                if (days > 0)
                {
                    parts.Add(English.Number("day", days, spellOut));
                }
            }
            if (interval.Hours > 0)
            {
                parts.Add(English.Number("hour", interval.Hours, spellOut));
            }
            if (interval.Minutes > 0)
            {
                parts.Add(English.Number("minute", interval.Minutes, spellOut));
            }
            if (interval.Seconds > 0)
            {
                parts.Add(English.Number("second", interval.Seconds, spellOut));
            }

            if (!parts.Any())
            {
                return "now";
            }
            return English.Conjoin("and", parts);
        }

        public static TimeSpan Multiply(this TimeSpan interval, int times) {
            var result = new TimeSpan(0);
            while (--times >= 0)
            {
                result += interval;
            }
            return result;
        }
    }
}
