using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Returns .Current and then calls .MoveNext()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static T Pop<T>(this IEnumerator<T> enumerator)
        {
            var res = enumerator.Current;
            enumerator.MoveNext();
            return res;
        }
    }
}
