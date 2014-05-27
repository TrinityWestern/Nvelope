using System;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope
{
    public static class GroupExtensions
    {
        /// <summary>
        /// Returns the first element each group having the largest value for fn
        /// NOTE: Only returns one value, even if 
        /// </summary>
        public static IEnumerable<TElement> HavingMax<TElement, TKey, TCompare>(
            this IEnumerable<IGrouping<TKey, TElement>> groups,
            Func<TElement, TCompare> fn)
        {
            foreach (var group in groups)
                yield return group.OrderByDescending(fn).First();
        }

        /// <summary>
        /// Returns the first element each group having the smallest value for fn
        /// </summary>
        public static IEnumerable<TElement> HavingMin<TElement, TKey, TCompare>(
            this IEnumerable<IGrouping<TKey, TElement>> groups,
            Func<TElement, TCompare> fn)
        {
            foreach (var group in groups)
                yield return group.OrderBy(fn).First();
        }

        /// <summary>
        /// Returns a list of all the first elements to return a distinct value for fn.
        /// Any subsequent elements that return the same value for fn will be left out
        /// </summary>
        /// <example>{"a", "bb", "b"}.HavingDistinct(s => s.Length) returns {"a","bb"}</example>
        /// <returns></returns>
        public static IEnumerable<T> HavingDistinct<T, TCompare>(
            this IEnumerable<T> source,
            Func<T, TCompare> fn)
        {
            var seenValues = new List<TCompare>();
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
