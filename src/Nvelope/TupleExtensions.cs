using System;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope
{
    public static class TupleExtensions
    {
        public static IEnumerable<KeyValuePair<T1, T2>> ToKeyValues<T1, T2>
            (this IEnumerable<Tuple<T1, T2>> tuples)
        {
            return tuples.Select(t => new KeyValuePair<T1, T2>(t.Item1, t.Item2));
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<T1> FirstCol<T1, T2, T3, T4>
            (this IEnumerable<Tuple<T1, T2, T3, T4>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<T1> FirstCol<T1, T2, T3>
            (this IEnumerable<Tuple<T1, T2, T3>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<T1> FirstCol<T1, T2>
            (this IEnumerable<Tuple<T1, T2>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }


    }
}
