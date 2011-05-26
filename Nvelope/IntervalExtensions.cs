using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope.Combinatorics;

namespace Nvelope
{
    public static class IntervalExtensions
    {
        // TODO: this method is rather inefficient. It's roughly O(n!)
        // it could probably be more like O(n) if you sorted the intervals
        // and then created your overlaps smartly, also the result would
        // be more correct because there wouldn't be any overlapping overlaps
        // Improve this method https://radar.twu.ca/issues/2995
        public static IEnumerable<Interval<T>> Overlaps<T>(
            this IEnumerable<Interval<T>> ranges) where T : IComparable<T>
        {
            return ranges.Combinations(2)
                .Select(c=>c.First().Overlap(c.Second()))
                .Where(d=>d!=null);
        }

        public static IEnumerable<Interval<T>> Gaps<T>(
            this IEnumerable<Interval<T>> ranges) where T : IComparable<T>
        {
            return ranges.Intervals((a, b) => a.Gap(b)).Where(d => d != null);
        }
    }
}
