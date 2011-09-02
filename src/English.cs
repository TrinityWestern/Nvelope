namespace Nvelope
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class English
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

        public static string Number(string noun, int count)
        {
            string number = count.ToString();
            if (WrittenNumerals.Keys.Contains(count)) {
                number = WrittenNumerals[count];
            }
            return number + " " + (count != 1 ? English.Pluralize(noun) : noun);
        }

        public static string Pluralize(string noun)
        {
            return noun + "s";
        }
    }
}
