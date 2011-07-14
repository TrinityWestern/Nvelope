//-----------------------------------------------------------------------
// <copyright file="FieldNotFoundException.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Indicates that reflection couldn't find a field
    /// </summary>
    [Serializable]
    public class FieldNotFoundException : Exception
    {
        #region These are just here for FxCop rules

        /// <summary>
        /// Initializes a new instance of the FieldNotFoundException class.
        /// The instance will be empty.
        /// </summary>
        public FieldNotFoundException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FieldNotFoundException class.
        /// Important properties in the instance will be empty.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public FieldNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FieldNotFoundException class.
        /// Important properties in the instance will be empty.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the
        /// current exception, or a null reference if no inner exception is specified.</param>
        public FieldNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FieldNotFoundException class from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        /// <remarks>This is also one of those constructors we're supposed to provide</remarks>
        protected FieldNotFoundException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
