using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nvelope.Combinatorics
{
    public static class CombinatoricsListExtensions
    {
        /// <summary>
        /// Get a list of all the k-permutations from list. (In a permutation, order matters, so
        /// this will return more results than combinations). K is the number of items in each combination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "I need to.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
            Justification = "k has a special mathematical meaning")]
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int k)
        {
            if (k == 0)
                return new[] { new T[0] };

            return list.SelectMany(t =>
                list.Except(t).Permutations(k - 1).Select(c => t.And(c)));
        }

        /// <summary>
        /// Get a list of all of the k-combinations from list. (In combinations, order doesn't matter, so 
        /// this will return fewer results than permutations). K is the number of items in each combination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="numToTake"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "I need to.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
            Justification = "k has a special mathematical meaning")]
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, int k)
        {
            if (k == 0)
                return new[] { new T[0] };

            return list.SelectMany((t, i) =>
                list.Skip(i + 1).Combinations(k - 1).Select(c => t.And(c)));
        }
    }
}
