using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class DictionaryListExtensions
    {
        public static IEnumerable<V> Select<K,V>(this IEnumerable<Dictionary<K,V>> dicts, K key)
        {
            foreach(var dict in dicts)
                yield return dict[key];
        }

        public static IEnumerable<V> Distinct<K, V>(this IEnumerable<Dictionary<K, V>> dicts, K key)
        {
            return dicts.Select(key).Distinct();
        }
    }
}
