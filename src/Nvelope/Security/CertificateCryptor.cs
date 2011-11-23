using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Nvelope.Security
{
    /// <summary>
    /// Implements symmetic encryption using a certificate. The certificate is referenced by name, and must
    /// be installed on the local machine
    /// </summary>
    public class CertificateCryptor : ISymmetricCryptor
    {
        public CertificateCryptor(X509Certificate2 certificate)
        {
            if (certificate == null)
                throw new CertificateNotFoundException("The supplied certificate was null");

            _certificate = certificate;
        }

        private X509Certificate2 _certificate;

        public string Decrypt(string cyphertext)
        {
            var sp = (RSACryptoServiceProvider)
                (_certificate.HasPrivateKey ? _certificate.PrivateKey : _certificate.PublicKey.Key);

            var input = Convert.FromBase64String(cyphertext);
            var output = sp.Decrypt(input, false);
            return Encoding.Default.GetString(output);
        }

        public string Encrypt(string plaintext)
        {
            var sp = (RSACryptoServiceProvider)_certificate.PublicKey.Key;

            var input = Encoding.Default.GetBytes(plaintext);
            var output = sp.Encrypt(input, false);
            return Convert.ToBase64String(output);
        }
    }
}
