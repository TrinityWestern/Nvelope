using System;
using System.Collections.Generic;
using System.Text;

namespace Nvelope.Collections
{
    /// <summary>
    /// This interface allows the implementing class to be used in various tree-based 
    /// data structures.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeable<T>
    {
        /// <summary>
        /// This function returns an enumeration (array, list, whatever) of the children
        /// of the object.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITreeable<T>> GetChildren();

        /// <summary>
        /// Returns the item itself
        /// </summary>
        T Item { get; }
    }
}
