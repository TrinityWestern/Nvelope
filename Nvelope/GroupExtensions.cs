using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class GroupExtensions
    {
        /// <summary>
        /// Returns the first element each group having the largest value for fn
        /// NOTE: Only returns one value, even if 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="groups"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IEnumerable<T> HavingMax<T, G, K>(this IEnumerable<IGrouping<G, T>> groups, Func<T, K> fn)
        {
            foreach (var group in groups)
                yield return group.OrderByDescending(fn).First();
        }

        /// <summary>
        /// Returns the first element each group having the smallest value for fn
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="groups"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IEnumerable<T> HavingMin<T, G, K>(this IEnumerable<IGrouping<G, T>> groups, Func<T, K> fn)
        {
            foreach (var group in groups)
                yield return group.OrderBy(fn).First();
        }

        /// <summary>
        /// Returns a list of all the first elements to return a distinct value for fn.
        /// Any subsequent elements that return the same value for fn will be left out
        /// </summary>
        /// <example>{"a", "bb", "b"}.HavingDistinct(s => s.Length) returns {"a","bb"}</example>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static IEnumerable<T> HavingDistinct<T, K>(this IEnumerable<T> source, Func<T, K> fn)
        {
            var seenValues = new List<K>();
            foreach (var t in source)
            {
                var k = fn(t);
                if (!seenValues.Contains(k))
                {
                    seenValues.Add(k);
                    yield return t;
                }
            }
        }
    }
}
