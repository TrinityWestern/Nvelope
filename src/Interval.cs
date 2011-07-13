using System;
using System.Linq;

namespace Nvelope
{
    public class Interval<T> : IEquatable<Interval<T>> where T : IComparable<T>
    {
        public T Start;
        public T End;

        public Interval(T start, T end)
        {
            this.Start = start;
            this.End = end;
        }
        public Interval(Interval<T> other)
        {
            this.Start = other.Start;
            this.End = other.End;
        }
        /// <summary>
        /// String representation of the range
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}—{1}", this.Start, this.End);
        }
        /// <summary>
        /// Test if range contains this item
        /// </summary>
        public bool Contains(T item)
        {
            if (this.Start.CompareTo(this.End) <= 0)
                return (this.Start.CompareTo(item) <= 0)
                    && (this.End.CompareTo(item) >= 0);
            else
                return (this.Start.CompareTo(item) >= 0)
                    && (this.End.CompareTo(item) <= 0);
        }
        /// <summary>
        /// Test if this interval includes the other
        /// </summary>
        /// <remarks>A contains B if A == B</remarks>
        public bool Contains(Interval<T> other)
        {
            var interval = this.Normalized();
            other = other.Normalized();

            return interval.Start.CompareTo(other.Start) <= 0
                && interval.End.CompareTo(other.End) >= 0;
        }

        /// <summary>
        /// Get an interval that goes in the opposite direction
        /// </summary>
        public Interval<T> Flip()
        {
            return new Interval<T>(this.End, this.Start);
        }
        public Interval<T> Overlap(Interval<T> other)
        {
            var interval = this.Normalized();
            other = other.Normalized();

            var start = interval.Start.And(other.Start).Max();
            var end = interval.End.And(other.End).Min();

            if (start.CompareTo(end) >= 0)
                return null;
            return new Interval<T>(start, end);
        }

        public Interval<T> Gap(Interval<T> other)
        {
            var interval = this.Normalized();
            other = other.Normalized();

            if (interval.End.CompareTo(other.Start) >= 0)
                return null;
            return new Interval<T>(this.End, other.Start);
        }

        public bool Equals(Interval<T> other)
        {
            return (this.Start.CompareTo(other.Start)
                | other.End.CompareTo(this.End)) == 0;
        }

        public Interval<T> Normalized()
        {
            if (this.Start.CompareTo(this.End) > 0)
                return this.Flip();
            return this;
        }

    }
}
