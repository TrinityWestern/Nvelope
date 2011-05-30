using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
