using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class CharListExtensions
    {
        /// <summary>
        /// Convert a list of chars back into a string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Read(this IEnumerable<char> source)
        {
            return new string(source.ToArray());
        }

        public static string Read(this char source)
        {
            return new string(source.List().ToArray());
        }

        /// <summary>
        /// Explicitly convert the string to IEnumerable{char}
        /// </summary>
        [Obsolete("Only used in a test with and obsolete method")]
        public static IEnumerable<char> Chars(this string source)
        {
            foreach (char c in source)
                yield return c;
        }

        /// <summary>
        /// Generates an infinite sequence of strings in a digits pattern - ie ('a'.Inc('b') -> (a,b,aa,ab,ba,bb,aaa ....)
        /// </summary>
        /// <param name="start">The first "digit" to use</param>
        /// <param name="end">The last "digit" to use</param>
        // TODO: I think the name of this method is confusing
        public static IEnumerable<string> Inc(this char start, char endOfRange)
        {
            var a = (int)start;
            var b = (int)endOfRange;
            var range = a.To(b).Select(i => ((char)i).Read());            
            foreach (var res in range)
                yield return res;
            var suffix = range.ToList();
            while (true)
            {
                var output = range.SelectMany(first => suffix.Select(rest => first + rest));
                foreach(var res in output)
                    yield return res;
                suffix = output.ToList();
            }

        }
    }
}
