using System;
using System.Collections.Generic;

namespace Nvelope
{
    public static class IndexingExtensions
    {
        /// <summary>
        /// Returns a dictionary of each item in the list indexed by some function
        /// NOTE: keySelector MUST return a unique value for each item in the list, or
        /// an exception will be thrown. If there's any doubt, use Index instead
        /// </summary>
        public static Dictionary<TKey, TValue> IndexFlat<TKey, TValue>(
            this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (var val in source)
                res.Add(keySelector(val), val);
            return res;
        }

        public static Dictionary<TKey, List<TValue>> Index<TKey, TValue>(
            this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            Dictionary<TKey, List<TValue>> res = new Dictionary<TKey, List<TValue>>();
            foreach (var val in source)
            {
                var key = keySelector(val);
                if (!res.ContainsKey(key))
                    res.Add(key, new List<TValue>());
                res[key].Add(val);
            }
            return res;
        }

        public static Dictionary<TKey, TValue> MapIndex<TKey, TValue>(
            this IEnumerable<TKey> source, Func<TKey, TValue> function)
        {
            Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
            foreach (var item in source)
                res.Add(item, function(item));
            return res;
        }
    }
}
