//-----------------------------------------------------------------------
// <copyright file="SerializationExtensions.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Helper methods for Serialization
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Retrieves a value from the SerializationInfo store.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to</typeparam>
        /// <param name="info">The serialization data</param>
        /// <param name="name">The name of the parameter</param>
        /// <returns>parameter with type T</returns>
        public static T GetValue<T>(this SerializationInfo info, string name)
        {
            return (T)info.GetValue(name, typeof(T));
        }
    }
}
