using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class HashSetExtensions
    {
        /// <summary>
        /// Wrapper around new HashSet(list)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> list)
        {
            return new HashSet<T>(list);
        }

        /// <summary>
        /// All the elements that are in one set or the other, but not both
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IEnumerable<T> SymmetricExcept<T>(this HashSet<T> a, HashSet<T> b)
        {
            var ea = a.Except(b);
            var eb = b.Except(a);
            return ea.Union(eb);
        }
    }
}
