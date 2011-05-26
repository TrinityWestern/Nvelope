using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Tests whether 2 objects are equal, but handles nulls gracefully
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        public static bool Eq(this object obj, object other)
        {
            // Handle nulls
            if (obj == null)
                return (other == null);     
            
            if (other == null)
                return false;

            return obj.Equals(other);
        }
    }
}
