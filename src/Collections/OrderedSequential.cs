using System;
using Nvelope;

namespace Nvelope.Collections
{
    public abstract class OrderedSequential<T> : Comparable<T>
    {
        public static T operator +(OrderedSequential<T> obj, int n)
        {
            return obj.Add(n);
        }
        public static T operator -(OrderedSequential<T> obj, int n)
        {
            return obj.Add(-n);
        }
        public static int operator -(OrderedSequential<T> obj1, T obj2)
        {
            return obj1.Difference(obj2);
        }
        protected abstract T Add(int n);
    }
}
