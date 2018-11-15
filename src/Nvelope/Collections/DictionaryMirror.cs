using System.Collections.Generic;

namespace Nvelope.Collections
{
    /// <summary>
    /// This establishes a one to one mapping between a's and b's.
    /// Say you had CheckBoxes and CalendarTags: given any checkbox
    /// you could find the associated tag; given any tag, you could
    /// find the assoicated checkbox.
    /// Technically: y(f(x)) = x
    /// </summary>
    /// <typeparam name="TFirst">The first type.</typeparam>
    /// <typeparam name="TSecond">The second type.</typeparam>
    public class DictionaryMirror<TFirst, TSecond>
    {
        private Dictionary<TFirst, TSecond> mapOne = new Dictionary<TFirst, TSecond>();
        private Dictionary<TSecond, TFirst> mapTwo = new Dictionary<TSecond, TFirst>();
        private HashSet<TFirst> SetOne = new HashSet<TFirst>();
        private HashSet<TSecond> SetTwo = new HashSet<TSecond>();

        /// <summary>
        /// Adds an association of one value to another into the one-to-one mapping.
        /// </summary>
        /// <param name="valueOne">A value of the first type.</param>
        /// <param name="valueTwo">A value of the second type.</param>
        public void Add(TFirst valueOne, TSecond valueTwo)
        {
            mapOne.Add(valueOne, valueTwo);
            mapTwo.Add(valueTwo, valueOne);
            SetOne.Add(valueOne);
            SetTwo.Add(valueTwo);
        }

        /// <summary>
        /// Gets the <see cref="TFirst"/> having the specified <see cref="TSecond"/> value.
        /// </summary>
        /// <value>
        /// The <see cref="TFirst"/>.
        /// </value>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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
