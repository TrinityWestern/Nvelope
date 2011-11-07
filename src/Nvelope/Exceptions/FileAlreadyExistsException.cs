//-----------------------------------------------------------------------
// <copyright file="FileAlreadyExistsException.cs" company="TWU">
// MIT Licenced
// </copyright>
//-----------------------------------------------------------------------

namespace Nvelope.Exceptions
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Tried to over-write a file but the file alreay exists
    /// </summary>
    [Serializable]
    public class FileAlreadyExistsException : IOException
    {
        public string FileName { get; set; }

        public FileAlreadyExistsException(string filename, string message)
            : base(message)
        {
            FileName = filename;
        }

        #region These are just here for FxCop rules

        /// <summary>
        /// Initializes a new instance of the FileAlreadyExistsException class.
        /// The instance will be empty.
        /// </summary>
        public FileAlreadyExistsException()
            : base()
        {
        }

        public FileAlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FileAlreadyExistsException class.
        /// Important properties in the instance will be empty.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the
        /// current exception, or a null reference if no inner exception is specified.</param>
        public FileAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FileAlreadyExistsException class from serialized data
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        /// <remarks>This is also one of those constructors we're supposed to provide</remarks>
        protected FileAlreadyExistsException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion        

    }
}
