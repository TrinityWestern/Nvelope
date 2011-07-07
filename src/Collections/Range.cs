//-----------------------------------------------------------------------
// <copyright file="Range.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// <para>A sequence of ordered items.</para>
    /// <para>This class is the collection class for IOrderedSequencial
    /// objects.</para>
    /// </summary>
    /// <typeparam name="T">An ordered type</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "We don't want to use the Collection suffix, it's silly and not really appropriate")]
    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes",
        Justification = "The only method that isn't callable is the non-" +
        "generic GetEnumerator, and it shouldn't be used because it's stupid")]
    public class Range<T> : IEnumerable, IEnumerable<T> where T : OrderedSequential<T>
    {
        /// <summary>
        /// Initializes a new instance of the Range class with properties
        /// </summary>
        /// <param name="first">first item in range</param>
        /// <param name="last">last item in range</param>
        public Range(T first, T last)
        {
            this.First = first;
            this.Last = last;
        }

        /// <summary>
        /// Gets or sets the first item in the range
        /// </summary>
        public T First { get; set; }

        /// <summary>
        /// Gets or sets the last item in the range
        /// </summary>
        public T Last { get; set; }

        /// <summary>
        /// <para>Gets the number of items in the sequence.</para>
        /// <para>The result is never negative.</para>
        /// </summary>
        public int Count
        {
            get
            {
                return Math.Abs(this.First - this.Last) + 1;
            }
        }

        /// <summary>
        /// String representation of the range
        /// </summary>
        /// <returns>string in format "Start—End"</returns>
        public override string ToString()
        {
            return this.First + "—" + this.Last;
        }

        /// <summary>
        /// Test if range contains this item
        /// </summary>
        /// <param name="item">item to check for</param>
        /// <returns>true if item is within the range</returns>
        public bool Contains(T item)
        {
            if (this.First.CompareTo(this.Last) <= 0)
            {
                return (this.First.CompareTo(item) <= 0)
                    && (this.Last.CompareTo(item) >= 0);
            }
            else
            {
                return (this.First.CompareTo(item) >= 0)
                    && (this.Last.CompareTo(item) <= 0);
            }
        }

        /// <summary>
        /// Get a range that goes in the opposite direction
        /// </summary>
        /// <returns>Flipped Range</returns>
        public Range<T> Flip()
        {
            return new Range<T>(this.Last, this.First);
        }

        /// <summary>
        /// Gets an enumerator that iterates from one end of the range to
        /// the other. It starts at the smaller end.
        /// </summary>
        /// <returns>An Enumerator of the generic type</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var current = this.First;
            while (current.CompareTo(this.Last) < 0)
            {
                yield return current;
                current = current + 1;
            }

            while (current.CompareTo(this.Last) > 0)
            {
                yield return current;
                current = current - 1;
            }

            yield return current;
        }

        /// <summary>
        /// Silly method required for the old .NET interface *rage*
        /// </summary>
        /// <returns>Some dumb enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (object item in this)
            {
                yield return item;
            }
        }
    }
}
