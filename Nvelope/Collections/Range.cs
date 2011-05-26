using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Collections
{
    /// <summary>
    /// A sequence of ordered items
    /// 
    /// This class is the collection class for IOrderedSequencial
    /// objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Range<T> : IEnumerable, IEnumerable<T> where T : OrderedSequential<T>
    {
        public T First;
        public T Last;

        public Range(T first, T last)
        {
            this.First = first;
            this.Last = last;
        }
        /// <summary>
        /// String representation of the range
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}—{1}", this.First, this.Last);
        }
        /// <summary>
        /// The number of items in the sequence
        /// 
        /// The result is never negative
        /// </summary>
        public int Count
        {
            get
            {
                return Math.Abs(First - Last) + 1;
            }
        }
        /// <summary>
        /// Test if range contains this item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            if (this.First.CompareTo(this.Last) <= 0)
                return (this.First.CompareTo(item) <= 0)
                    && (this.Last.CompareTo(item) >= 0);
            else
                return (this.First.CompareTo(item) >= 0)
                    && (this.Last.CompareTo(item) <= 0);

        }

        /// <summary>
        /// Get a range that goes in the opposite direction
        /// </summary>
        public Range<T> Flip()
        {
            return new Range<T>(this.Last, this.First);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var current = this.First;
            while (current.CompareTo(this.Last) < 0) {
                yield return current;
                current = current + 1;
            }
            while (current.CompareTo(this.Last) > 0) {
                yield return current;
                current = current - 1;
            }
            yield return current;
        }
        /// <summary>
        /// Silly method required for the old .NET interface
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (object item in this) yield return item;
        }
    }
}
