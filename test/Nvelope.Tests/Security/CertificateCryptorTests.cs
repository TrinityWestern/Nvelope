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
        public void Works()
        {
            var str = "The quick brown fox jumps over the lazy dog.";
            var cryptor = GetCryptor();
            var cipher = cryptor.Encrypt(str);
            Assert.AreNotEqual(cipher, str);
            Assert.AreEqual(str, cryptor.Decrypt(cipher));
        }

        [Test]
        public void ThrowsExceptionIfNoCert()
        {
            Assert.Throws<CertificateNotFoundException>(() => new CertificateCryptor(null));
        }
    }
}
