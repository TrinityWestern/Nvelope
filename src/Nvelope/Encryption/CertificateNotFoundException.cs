using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Encryption
{
    public class CertificateNotFoundException : Exception
    {
        public CertificateNotFoundException(string message)
            : base(message)
        { }
    }
}
