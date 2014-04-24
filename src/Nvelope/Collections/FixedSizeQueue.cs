using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Collections
{
    /// <summary>
    /// A queue of a fixed size - once the max size is reached, items are dropped off the end of the queue when
    /// new items are added
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FixedSizeQueue<T> : IEnumerable<T>
    {
        public FixedSizeQueue(int size)
        {
            if (size < 1)
                throw new InvalidOperationException("You can't have a queue smaller than 1 element");

            Size = size;
            q = new Queue<T>(Size);
        }

        public int Size { get; protected set; }
        protected Queue<T> q;
        private object _lck = new object();

        public int Count { get { return q.Count; } }

        public void Enqueue(T item)
        {
            lock (_lck)
            {
                while (Count > (Size - 1))
                    q.Dequeue();
                q.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            return q.Dequeue();
        }

        public void Clear()
        {
            lock (_lck)
                q.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return q.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return q.GetEnumerator();
        }
    }
}
