//-----------------------------------------------------------------------
// <copyright file="ConversionException.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Nvelope;

    /// <summary>
    /// Indicates that an error has occured while doing a type conversion
    /// </summary>
    [Serializable]
    public class ConversionException : Exception
    {
        #region These are just here for CA1032

        /// <summary>
        /// Initializes a new instance of the ConversionException class.
        /// The instance will be empty.
        /// </summary>
        public ConversionException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConversionException class.
        /// Important properties in the instance will be empty.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ConversionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ConversionException class.
        /// Important properties in the instance will be empty.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the
        /// current exception, or a null reference if no inner exception is specified.</param>
        public ConversionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion

        #region These are the constructors we actually plan to use

        /// <summary>
        /// Initializes a new instance of the ConversionException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="expectedType">The type that the value was trying to be converted to</param>
        /// <param name="value">The value that was trying to be converted</param>
        public ConversionException(string message, Type expectedType, object value)
            : base(message)
        {
            this.ExpectedType = expectedType;
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the ConversionException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="expectedType">The type that the value was trying to be converted to</param>
        /// <param name="value">The value that was trying to be converted</param>
        /// <param name="innerException">The exception that is the cause of the
        /// current exception, or a null reference if no inner exception is specified.</param>
        public ConversionException(string message, Type expectedType, object value, Exception innerException)
            : base(message, innerException)
        {
            this.ExpectedType = expectedType;
            this.Value = value;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ConversionException class from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        /// <remarks>This is also one of those constructors we're supposed to provide</remarks>
        protected ConversionException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ExpectedType = info.GetValue<Type>("ExpectedType");
            this.Value = info.GetValue<object>("Value");
        }

        /// <summary>
        /// Gets or sets the type that the value was trying to be converted to
        /// </summary>
        public Type ExpectedType { get; protected set; }

        /// <summary>
        /// Gets or sets the value that was trying to be converted
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Sets the SerializationInfo with information about the exception.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
            info.AddValue("ExpectedType", this.ExpectedType);
            info.AddValue("Value", this.Value);
        }
    }
}
