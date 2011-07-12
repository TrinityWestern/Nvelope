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
    public class DictionaryMirror<TFirst, TSecond>
    {
        private Dictionary<TFirst, TSecond> mapOne = new Dictionary<TFirst, TSecond>();
        private Dictionary<TSecond, TFirst> mapTwo = new Dictionary<TSecond, TFirst>();
        private HashSet<TFirst> SetOne = new HashSet<TFirst>();
        private HashSet<TSecond> SetTwo = new HashSet<TSecond>();

        public void Add(TFirst valueOne, TSecond valueTwo)
        {
            mapOne.Add(valueOne, valueTwo);
            mapTwo.Add(valueTwo, valueOne);
            SetOne.Add(valueOne);
            SetTwo.Add(valueTwo);
        }

        public TFirst this[TSecond value]
        {
            get { return mapTwo[value]; }
        }

        public TSecond this[TFirst value]
        {
            get { return mapOne[value]; }
        }

        public IEnumerable<TFirst> AllFirst() 
        {
            return SetOne;
        }

        public IEnumerable<TSecond> AllSecond() 
        {
            return SetTwo;
        }
    }
}
