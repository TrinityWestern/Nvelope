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
        #region These are just here for FxCop rules

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
        }

        #endregion
    }
}
