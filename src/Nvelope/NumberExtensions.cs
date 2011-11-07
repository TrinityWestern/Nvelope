using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nvelope
{
    public static class NumberExtensions
    {
        public static bool IsEven(this int num)
        {
            return num % 2 == 0;
        }

        public static bool IsOdd(this int num)
        {
            return !num.IsEven();
        }
        /// <summary>
        /// The  integer modulo where the divisor's sign is maintained,
        /// not the the dividends.
        /// 
        /// If the dividend's sign is maintained, it is typically called
        /// the remainder.
        /// 
        /// For example, (-3).Mod(5) == 2, this is different from
        /// the `%` in C#.
        /// </summary>
        /// <returns>A number between 0 and b</returns>
        public static int Mod(this int a, int b)
        {
            if (a == -b) return 0;
            if (b < 0) return -a.Mod(-b);
            if (a < 0) return b + (a % b);
            else return a % b;
        }
        /// <summary>
        /// Integer division that always rounds down
        /// 
        /// Equivilant to a/b in many languages. It complements the
        /// modulo operation.
        /// </summary>
        public static int DivDown(this int a, int b)
        {
            var result = a / b;
            if (a % b < 0) result--;
            return result;
        }

        /// <summary>
        /// Perform some action num times
        /// </summary>
        /// <param name="num"></param>
        /// <param name="action"></param>
        public static void Times(this int num, Action action)
        {
            for (int i = 0; i < num; i++)
                action();
        }

        /// <summary>
        /// Get a list of numbers from start to end (inclusive), incrementing by increment
        /// </summary>        
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="increment">defaults to 1 or -1</param>
        /// <returns></returns>
        public static IEnumerable<int> To(this int start, int end, int? increment=null)
        {
            int incr = increment ?? ((start < end)? 1:-1);
            if (incr == 0)
                throw new ArgumentOutOfRangeException("Increment cannot be 0");

            if (incr > 0)
                if (end < start)
                    throw new ArgumentOutOfRangeException("End must be greater than start if increment is positive");
                else
                    for (int i = start; i <= end; i += incr)
                        yield return i;
            else
                if (start < end)
                    throw new ArgumentOutOfRangeException("Start must be greater than end if increment is negative");
                else
                    for (int i = start; i >= end; i += incr)
                        yield return i;
        }

        /// <summary>
        /// Starting at start, generate an infinite sequence of numbers,
        /// incrementing by increment. WARNING: Generates an infinite (or nearly infinite)
        /// sequence, so call Take() on this or something, or your program will be here
        /// forever.
        /// </summary>
        public static IEnumerable<int> Inc(this int start, int? increment = 1)
        {
            if (increment >= 0)
                return start.To(int.MaxValue, increment);
            else
                return start.To(int.MinValue, increment);
        }

        /// <summary>
        /// A wrapper around Decimal.Round
        /// </summary>
        public static decimal RoundTo(this decimal number, int decimalPlaces = 0, MidpointRounding roundingRule = MidpointRounding.AwayFromZero)
        { 
            return Decimal.Round(number, decimalPlaces, roundingRule);
        }

        /// <summary>
        /// Formats a rate as a percentage.
        /// </summary>
        /// <param name="rate">The rate specified as decimal.</param>
        /// <returns>String with the percentage, plus a percent sign.</returns>
        public static string AsPercentage(this decimal rate)
        {
            return (rate * 100).ToString("0.00", CultureInfo.CurrentCulture) + "%";
		}

        /// <summary>
        /// Is number the same as other, to within precision
        /// ie, 1.01m.IsAbout(1.0m, .1m) means "is 1.01 equal 1.0 to within .1?"
        /// </summary>
        public static bool IsAbout(this decimal number, decimal other, decimal precision)
        {
            return Math.Abs(number - other) < precision;
        }

        public static string Print(this decimal o)
        {
            // Decimals don't do ToString in a reasonable way
            // It's really irritating
            // So chop off the trailing .0000 if it has it
            var parts = o.ToStringN().Split('.');
            if (parts.Count() == 1)
                return parts.First();
            else if (parts.Second().All(c => c == '0')) // if it's all 0s, drop the decimal part
                return parts.First();
            else
                return parts.First() + "." + Regex.Replace(parts.Second(), "0*$", ""); // trim off trailing 0s
        }
    }
}
