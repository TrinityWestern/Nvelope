using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Exceptions
{
    /// <summary>
    /// Indicates that reflection couldn't find a field
    /// </summary>
    public class FieldNotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="expectedField"></param>
        public FieldNotFoundException(object obj, string expectedField)
            : base("Could not find a field called " + expectedField + " on the object")
        {
            Object = obj;
            ExpectedField = expectedField;
        }

        public FieldNotFoundException(object obj, string expectedField, string message) : base(message)
        {
            Object = obj;
            ExpectedField = expectedField;
        }

        /// <summary>
        /// The object that we expected to find a field on
        /// </summary>
        public object Object;
        /// <summary>
        /// The field we expected to find
        /// </summary>
        public string ExpectedField;
    }
}
