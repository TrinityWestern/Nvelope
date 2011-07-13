using System;
using System.Diagnostics.CodeAnalysis;

namespace Nvelope
{
    /// <summary>
    /// This is a stupic class to work around the fact that C# has
    /// an awfully complicated way of doing comparisons and requires
    /// you to be explicit until you're blue in the face.
    /// </summary>
    public abstract class Comparable<T> : IComparable<T>, IComparable
    {
        public static bool operator <(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) < 0;
        }
        public static bool operator >(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) > 0;
        }
        public static bool operator ==(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) == 0;
        }
        public static bool operator !=(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) != 0;
        }
        public static bool operator <=(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) <= 0;
        }
        public static bool operator >=(Comparable<T> obj1, Comparable<T> obj2)
        {
            return Compare(obj1, obj2) >= 0;
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes",
            Justification = "FxCop wanted this method public here as an alternet to the operator versions.")]
        public static int Compare(Comparable<T> obj1, Comparable<T> obj2)
        {
            if (object.ReferenceEquals(obj1, obj2)) return 0;
            if (object.ReferenceEquals(obj1, null)) return -1;
            if (object.ReferenceEquals(obj2, null)) return -2;
            return obj1.CompareTo(obj2);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is T))
                throw new NotImplementedException("Unable to compare types");
            return CompareTo((T)obj);
        }

        public int CompareTo(T other)
        {
            if (object.ReferenceEquals(other, null))
                return -1;
            return this.Difference(other);
        }
        /// <summary>
        /// This function defines the way that the - operator works.
        /// It is also used to do comparisons bewteen objects
        /// </summary>
        /// <returns>positive if `this` is greater than `other`</returns>
        public abstract int Difference(T other);
        public abstract override int GetHashCode();

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
            Justification = "The cast is necessary to do value comparison.")]
        public override bool Equals(object obj)
        {
            if (!(obj is Comparable<T>)) return false;
            return this == (Comparable<T>)obj;
        }
    }
}
