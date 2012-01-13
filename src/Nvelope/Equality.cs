using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    /// <summary>
    /// A quick implementation of IEqualityComparer. 
    /// This is handy if you've got some methods lying around to do comparison, but don't
    /// want to write an entire new class to do it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Equality<T> : IEqualityComparer<T>
    {
        public Equality(Func<T, T, bool> equals, Func<T, int> getHashCode = null)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        private Func<T, T, bool> _equals;
        private Func<T, int> _getHashCode;

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (_getHashCode != null)
                return _getHashCode(obj);
            else
                return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Support methods for equality
    /// </summary>
    public static class Equality
    {
        public static Equality<T> MakeComparer<T>(Func<T, T, bool> equals, Func<T, int> getHashCode = null)
        {
            return new Equality<T>(equals, getHashCode);
        }
    }
}
