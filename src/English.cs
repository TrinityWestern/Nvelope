namespace Nvelope
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class English
    {
        public static Dictionary<int, string> WrittenNumerals = new Dictionary<int, string> {
            {0, "no"},
            {1, "one"},
            {2, "two"},
            {3, "three"}
        };

        public static string Conjoin(string conjuction, IEnumerable<string> parts)
        {
            if (!parts.Any())
            {
                return "";
            }
            if (parts.Count() == 1)
            {
                return parts.First();
            }
            if (parts.Count() == 2)
            {
                return parts.First() + " " + conjuction + " " + parts.Second();
            }
            var firstBit = parts.Slice(0, -1);
            return firstBit.Join(", ") + ", " + conjuction + " " + parts.Last();
        }

        public static string Number(string noun, int count, bool spellOut = false)
        {
            string number = count.ToString();
            if (spellOut && WrittenNumerals.Keys.Contains(count)) {
                number = WrittenNumerals[count];
            }
            return number + " " + (count != 1 ? English.Pluralize(noun) : noun);
        }

        public static string Pluralize(string noun)
        {
            return noun + "s";
        }

        public static string DescribeByMonth(this Interval<DateTime> when)
        {
            // We're going to ad an extra day here because a span of January 5
            // to February 5 is really 32 days.
            var start = when.Start;
            var end = when.End.AddDays(1);

            var years = end.Year - start.Year;
            var months = end.Month - start.Month;
            var days = end.Day - start.Day;

            if (months < 0)
            {
                months += 12;
                years -= 1;
            }
            if (days < 0)
            {
                // It's important that we use the days in month of the starting
                // date time, otherwise the value in days might still be below
                // zero after this.
                days += when.Start.DaysInMonth();
                months -= 1;
            }
            // also, you need to use the days in the start month here too.
            if ((days * 2) >= when.Start.DaysInMonth())
            {
                months += 1;
            }

            var approx = (days == 0) ? "" : "~";

            if (years > 0)
            {
                var str = approx + English.Number("year", years);
                if (months > 0)
                {
                    str += " and " + English.Number("month", months);
                }
                return str;
            }
            return approx + English.Number("month", months);
        }
    }
}
