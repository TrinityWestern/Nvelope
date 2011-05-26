using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class TupleExtensions
    {
        public static IEnumerable<KeyValuePair<A, B>> ToKeyValues<A, B>
            (this IEnumerable<Tuple<A, B>> tuples)
        {
            return tuples.Select(t => new KeyValuePair<A, B>(t.Item1, t.Item2));
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<A> FirstCol<A, B, C, D>(this IEnumerable<Tuple<A, B, C, D>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<A> FirstCol<A, B, C>(this IEnumerable<Tuple<A, B, C>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }

        /// <summary>
        /// Selects just the Item1s of the tuples
        /// </summary>
        public static IEnumerable<A> FirstCol<A, B>(this IEnumerable<Tuple<A, B>> tuples)
        {
            return tuples.Select(t => t.Item1);
        }


    }
}
