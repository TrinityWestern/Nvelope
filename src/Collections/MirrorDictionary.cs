using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Collections
{
    /// <summary>
    /// This establishes a one to one mapping between a's and b's.
    /// Say you had CheckBoxes and CalendarTags: given any checkbox
    /// you could find the associated tag; given any tag, you could
    /// find the assoicated checkbox.
    /// 
    /// Technically: y(f(x)) = x
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class MirrorDictionary<A, B>
    {
        private Dictionary<A, B> mappingA = new Dictionary<A, B>();
        private Dictionary<B, A> mappingB = new Dictionary<B, A>();
        private List<A> aList = new List<A>();
        private List<B> bList = new List<B>();

        public void Add(A a, B b)
        {
            mappingA.Add(a, b);
            mappingB.Add(b, a);
            aList.Add(a);
            bList.Add(b);
        }

        public A this[B b]
        {
            get { return mappingB[b]; }
        }

        public B this[A a]
        {
            get { return mappingA[a]; }
        }

        public List<A> AllA() 
        {
            return aList;
        }

        public List<B> AllB() 
        {
            return bList;
        }
    }
}
