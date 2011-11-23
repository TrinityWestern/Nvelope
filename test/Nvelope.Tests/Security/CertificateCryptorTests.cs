using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Security;
using System.Security.Cryptography.X509Certificates;

namespace Nvelope.Tests.Encryption
{
    [TestFixture]
    public class CertificateCryptorTests : SymmetricTests
    {
        public override ISymmetricCryptor GetCryptor()
        {
            return new CertificateCryptor(Certificates.UserCert());
        }

        [Test]
        public void ThrowsExceptionIfNoCert()
        {
            Assert.Throws<CertificateNotFoundException>(() => new CertificateCryptor(null));
        }
    }
}
