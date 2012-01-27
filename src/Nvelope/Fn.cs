using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class Fn
    {
        /// <summary>
        /// Apply all of the supplied functions, one after the other, to the supplied input.
        /// Equivalent to fns[2](fns[1](fns[0](obj)))... but more readable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fns"></param>
        /// <returns></returns>
        public static T Thread<T>(T obj, params Func<T, T>[] fns)
        {
            if (fns == null)
                return obj;

            T cur = obj;
            foreach (var fn in fns)
                cur = fn(cur);

            return cur;
        }
    }
}
