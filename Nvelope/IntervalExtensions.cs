using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope.Combinatorics;

namespace Nvelope
{
    public static class IntervalExtensions
    {
        /// <summary>
        /// Merge overlapping intervals together. Produces the smallest
        /// enumeration possible to represent a series of intervals.
        /// </summary>
        public static IEnumerable<Interval<T>> Merge<T>(
            this IEnumerable<Interval<T>> ranges) where T : IComparable<T>
        {
            var sorted = new Stack<Interval<T>>(ranges.OrderByDescending(r => r.Start));
            if (sorted.Count < 1) yield break;

            var next = new Interval<T>(sorted.Pop());
            while (sorted.Count > 0) {
                if (sorted.Peek().Start.CompareTo(next.End) >= 0) {
                    yield return next;
                    next = new Interval<T>(sorted.Pop());
                } else {
                    next.End = sorted.Pop().End;
                }
            }
            yield return next;
        }

        /// <summary>
        /// Returns the intervals of overlapping intervals in an enumeration.
        /// </summary>
        /// <remarks>This is much faster than doing your own thing with
        /// combinatorics. At the moment, this might not produce the smallest
        /// series of intervals. Filter it with Merge to get that result.
        /// </remarks>
        /// <returns>Overlapping intervals (empty if no overlaps)</returns>
        public static IEnumerable<Interval<T>> Overlaps<T>(
            this IEnumerable<Interval<T>> ranges) where T : IComparable<T>
        {
            var sorted = new Stack<Interval<T>>(ranges.OrderByDescending(r => r.Start));
            
            if (sorted.Count < 2) yield break;

            Interval<T> bottom = new Interval<T>(sorted.Pop());

            while (sorted.Count > 0) {
                if (sorted.Peek().Start.CompareTo(bottom.End) >= 0) {
                    bottom = new Interval<T>(sorted.Pop());
                } else {
                    yield return sorted.Peek().Overlap(bottom);
                    bottom.End = sorted.Pop().End;
                }
            }
        }

        public static IEnumerable<Interval<T>> Gaps<T>(
            this IEnumerable<Interval<T>> ranges) where T : IComparable<T>
        {
            return ranges.Intervals((a, b) => a.Gap(b)).Where(d => d != null);
        }
    }
}
