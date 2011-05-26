using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Exceptions
{
    /// <summary>
    /// Indicates that an error has occured while doing a type conversion
    /// </summary>
    public class ConversionException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConversionException(string message, Type expectedType, object value)
            : this(message, expectedType, value, null)
        {
            ExpectedType = expectedType;
            Value = value;
        }

        public ConversionException(string message, Type expectedType, object value, Exception innerException)
            : base(message, innerException)
        {
        }

        public Type ExpectedType { get; protected set; }
        public object Value { get; protected set; }
    }
}
