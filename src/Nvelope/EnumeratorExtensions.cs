using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Calls .MoveNext and returns .Current
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <param name="endValue">The value to return when the end of the enumerator is reached. Defaults to
        /// default(T)</param>
        /// <returns></returns>
        public static T Pop<T>(this IEnumerator<T> enumerator, T endValue = default(T))
        {
            if (enumerator.MoveNext())
                return enumerator.Current;
            else
                return endValue;
        }
    }
}
