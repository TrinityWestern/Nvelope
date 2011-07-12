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
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryMirror<TKey, TValue>
    {
        private Dictionary<TKey, TValue> mappingA = new Dictionary<TKey, TValue>();
        private Dictionary<TValue, TKey> mappingB = new Dictionary<TValue, TKey>();
        private List<TKey> aList = new List<TKey>();
        private List<TValue> bList = new List<TValue>();

        public void Add(TKey key, TValue value)
        {
            mappingA.Add(key, value);
            mappingB.Add(value, key);
            aList.Add(key);
            bList.Add(value);
        }

        public TKey this[TValue value]
        {
            get { return mappingB[value]; }
        }

        public TValue this[TKey value]
        {
            get { return mappingA[value]; }
        }

        public IEnumerable<TKey> Keys() 
        {
            return aList;
        }

        public IEnumerable<TValue> Values() 
        {
            return bList;
        }
    }
}
