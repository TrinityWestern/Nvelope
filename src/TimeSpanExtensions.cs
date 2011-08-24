using System;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Rounding conversion of TimeSpan to Years
        /// </summary>
        public static int RoundToYears(this TimeSpan source)
        {
            return (source.TotalDays / 365).ConvertTo<decimal>().RoundTo().ConvertTo<int>();
        }

        /// <summary>
        /// Get a human-friendly approximate description of the length of the timespan
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string DescribeLength(this TimeSpan interval)
        {
            const double second = 1000.0;
            const double minute = 60 * second;
            const double hour = 60 * minute;
            const double day = 24 * hour;

            // A mapping of a lower limit (inclusive) of milliseconds to a function
            // that prints out a description of the time.
            // How this works:
            // The algorithm finds the smallest key where the number is greater than
            // or equal to the number of milliseconds in the timespan, then uses the
            // corresponding function to print out a description of the time
            var printers = new Dictionary<double, Func<TimeSpan, string>>()
            {        
                { 1.1, t => "1 millisecond"},
                { second * .9, t => t.Milliseconds + " milliseconds"},
                { second * 1.5, t => "1 second"},
                { second * 50, t => t.Seconds + " seconds"},
                { minute * 1.5, t => "1 minute"},
                { minute * 50, t => t.Minutes + " minutes"},
                { minute * 70, t => "1 hour"},
                { minute * 90, t => t.Minutes + " minutes"},
                { hour * 23, t => t.Hours + " hours"},
                { hour * 47, t => "1 day"},
                { day * 6, t => t.Days + " days"},
                { day * 8, t => "1 week"},
                { day * 24, t => t.Days / 7 + " weeks"},
                { day * 34, t => "1 month"},
                { day * 350, t => t.Days / 30 + " months"},
                { day * 380, t => "1 year"},
                { day * 715, t => t.Days / 30 + " months"},
                { day * 740, t => "2 years"},
                { day * 1080, t => t.Days / 30 + " months"},
                { day * 1110, t => "3 years"},
                { day * 1445, t => t.Days / 30 + " months"},
                {Double.MaxValue, t => t.Days / 365 + " years"}
            };

            var applicableKV = printers.OrderBy(kv => kv.Key)
                .First(kv => kv.Key >= interval.TotalMilliseconds);
            var printer = applicableKV.Value;

            return printer(interval);
        }
    }
}
