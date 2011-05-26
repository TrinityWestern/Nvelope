using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class IComparableExtensions
    {
        /// <summary>
        /// Returns true if dt is between start and end
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between<T>(this T  dt, T start, T end) where T:IComparable<T>
        {
            // Don't trust them - if they pass in start and end reversed, fix them
            var borders = start.And(end).Sort();
            start = borders.First();
            end = borders.Last();
            
            return dt.CompareTo(start) >= 0 && dt.CompareTo(end) <= 0;
        }
    }
}
