// -----------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="TWU">
// </copyright>
// -----------------------------------------------------------------------

namespace Nvelope
{
    using System;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// Some helpers for casting objects using built-in CLR casting operators
    /// </summary>
    /// <remark>
    /// In order to cast from one object to another, they must have
    /// either implicit or explicit cast operators defined. See
    /// http://msdn.microsoft.com/en-us/library/39bb81c3.aspx
    /// </remark>
    public static class TypeExtensions
    {
        /// <summary>
        /// The two operator methods used in .Net for casting, we evaluate them
        /// in this order
        /// </summary>
        private static string[] castOperators = new string[] { "op_Implicit", "op_Explicit" };

        /// <summary>
        /// Gets the most appropriate cast method from one type to another
        /// </summary>
        /// <param name="from">Type of initial object</param>
        /// <param name="to">Type of casted object</param>
        /// <returns>MethodInfo or null if no such method exists</returns>
        public static MethodInfo GetCastMethod(Type from, Type to)
        {
            var bind = BindingFlags.Static | BindingFlags.Public;
            var pm = new ParameterModifier[0];
            var target = new Type[] { from };

            foreach (var type in new Type[] { from, to })
            {
                foreach (var method in castOperators)
                {
                    var meth = type.GetMethod(method, bind, null, target, pm);
                    if (meth != null)
                    {
                        return meth;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Cast an object from one type to the other using .Net conversion
        /// </summary>
        /// <param name="t">Type to convert to</param>
        /// <param name="o">object to covert</param>
        /// <returns>Instance of type or null if object was null to start</returns>
        /// <exception cref="InvalidCastException">Thrown if conversion fails</exception>
        /// <remarks>
        /// There's a similar ConvertTo method in this library that is much
        /// more complicated and doesn't allow for user defined conversions
        /// </remarks>
        public static object DynamicCast(this Type targetType, object value)
        {
            // We can't handle null conversions
            if (value == null)
            {
                return value;
            }

            // First try to use the casting method for .Net's built-in type
            try
            {
                return Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
            }
            catch (InvalidCastException)
            {
            }

            // Fall back to user-defined operators
            var meth = GetCastMethod(value.GetType(), targetType);

            if (meth == null)
            {
                throw new InvalidCastException("Invalid Cast.");
            }

            return meth.Invoke(null, new object[] { value });
        }
    }
}
