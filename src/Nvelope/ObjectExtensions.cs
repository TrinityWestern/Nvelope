using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Data;
using Nvelope.Reflection;
using System.Collections;
using System.Collections.Generic;

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

        /// <summary>
        /// Inverse of Eq
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Neq(this object obj, object other)
        {
            return !obj.Eq(other);
        }

        /// <summary>
        /// Like ToString, but it handles nulls and gives nicer results for
        /// some objects.
        /// </summary>
        /// <remarks>There should be no other implementations of Print, because we want the
        /// thing to behave polymorphically, and there's no assurance of that unless
        /// we centralize here</remarks>
        public static string Print(this object o)
        {
            // This function should also work polymorphically
            // Sometimes, we've got variables of type object, but we want them to print 
            // nicer than ToString() for their type (ie, for decimals, ToString() works stupidly)
            // So we do shotgun polymorphism here to take care of that, since we can't 
            // hack into the original types to override their ToString methods

            if (o == null)
                return o.ToStringN();

            var type = o.GetType();

            if (type == typeof(decimal))
                return ((decimal)o).PrintDecimal(); // Decimals don't do ToString in a reasonable way
            else if (type == typeof(string))
                return o.ToStringN(); // This is to prevent the compiler from calling the IEnumerable<char> verison for strings
            else if (type == typeof(NameValueCollection))
                return ((NameValueCollection)o).ToDictionary().Print();
            else if (type.Implements<IDictionary>())
                return ((IDictionary)o).PrintDict();
            else if (type.Implements<IEnumerable>())
                return "(" + ((IEnumerable)o).ToIEnumerableObj().Select(t => t.Print()).Join(",") + ")";
            else if (o is Match)
            {
                var groups = ((Match)o).Groups.ToList().ToList();
                return groups.Select(g => g.ToString()).Print();

                // We can't do this, because the first group of a Match might be
                // the Match itself - that would get us into an infinite loop
                //return ((Match)o).Groups.ToList().Print(); // Regex group
            }
            else if (o is Capture)
                return ((Capture)o).Value; // Regex capture

            else if (o is DataTable)
                return "(" + ((DataTable)o).Rows.ToList().Select(l => l.Print()).Join(",") + ")";
            else if (o is DataRow)
                return ((DataRow)o).ToDictionary().Print();
            else
                return o.ToStringN();
        }
    }
}