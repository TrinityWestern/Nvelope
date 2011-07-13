using System;

namespace Nvelope.IO
{
    public class ExternalProcessException : Exception
    {
        public ExternalProcessException(string message)
            : base(message)
        { }

        public ExternalProcessException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
