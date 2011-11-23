using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Encryption;
using System.Security.Cryptography.X509Certificates;

namespace Nvelope.Tests.Encryption
{
    [TestFixture]
    public class SymmetricCertificateTests : SymmetricTests
    {
        public override ISymmetric GetCryptor()
        {
            var certLocation = StoreLocation.CurrentUser;
            var certSubj = "";

            // For testing, we'll try to get a cert from the user's local store
            X509Store store = new X509Store(certLocation);
            store.Open(OpenFlags.ReadOnly);
            if (store.Certificates.Count > 0)
                certSubj = store.Certificates[0].Subject;
            else
                throw new CertificateNotFoundException("We couldn't set up the test because we couldn't find ANY certificates on the local machine to test with");

            return new SymmetricCertificate(certSubj, certLocation);
        }

        [Test]
        public void ThrowsExceptionIfNoCert()
        {
            var cryptor = new SymmetricCertificate("foosums");
            Assert.Throws<CertificateNotFoundException>(() => cryptor.Encrypt("abc"));
        }
    }
}
