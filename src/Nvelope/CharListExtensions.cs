using System;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope
{
    /// <summary>
    /// Provides extensions for working with char lists
    /// </summary>
    public static class CharListExtensions
    {
        /// <summary>
        /// Convert a list of chars back into a string
        /// </summary>
        /// <param name="source">The source as a list of chars.</param>
        /// <returns>A <see cref="System.String" /> assembled from a list of chars.</returns>
        public static string Read(this IEnumerable<char> source)
        {
            return new string(source.ToArray());
        }

        /// <summary>
        /// Convert a single char to a string.
        /// </summary>
        /// <param name="source">The source as a char.</param>
        /// <returns>A <see cref="System.String" /> assembled from a char.</returns>
        public static string Read(this char source)
        {
            return new string(source.List().ToArray());
        }

        /// <summary>
        /// Generates an infinite sequence of strings in a digits pattern - ie ('a'.Inc('b') -&gt; (a,b,aa,ab,ba,bb,aaa ....)
        /// </summary>
        /// <param name="start">The first "digit" to use as char</param>
        /// <param name="endOfRange">The end of the range "digit" to use as char.</param>
        /// <returns>A list of <see cref="System.String" />s that follow the sequence.</returns>
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
