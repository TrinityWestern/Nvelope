
namespace Nvelope.Collections
{
    public abstract class OrderedSequential<T> : Comparable<T>
    {
        protected abstract T Add(int value);

        public static T operator +(OrderedSequential<T> obj, int value)
        {
            return obj.Add(value);
        }

        public T Subtract(int value)
        {
            return this.Add(-value);
        }

        public static T operator -(OrderedSequential<T> obj, int value)
        {
            return obj.Subtract(value);
        }

        public int Subtract(T value)
        {
            return this.Difference(value);
        }

        public static int operator -(OrderedSequential<T> obj, T value)
        {
            return obj.Subtract(value);
        }


    }
}
