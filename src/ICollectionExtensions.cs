using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Creates a new ICollection from a single item
        /// </summary>
        /// <remarks>This is an alternative to the List() method that turns a single
        /// item into a list. It exists because sometimes we use ICollections instead
        /// of lists (for example, in certain places in Lasy)</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ICollection<T> Coll<T>(this T item)
        {
            return new T[] { item };
        }
    }
}
