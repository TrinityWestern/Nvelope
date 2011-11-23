using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace Nvelope.Encryption
{
    /// <summary>
    /// Implements symmetic encryption using a certificate. The certificate is referenced by name, and must
    /// be installed on the local machine
    /// </summary>
    public class SymmetricCertificate : ISymmetric
    {
        public SymmetricCertificate(string certificateSubject, StoreLocation certificateStore = StoreLocation.LocalMachine)
        {
            _certSubject = certificateSubject;
            _certLocation = certificateStore;
        }

        private StoreLocation _certLocation;
        private string _certSubject;

        private X509Certificate2 _getCert()
        {
            var certStore = new X509Store(_certLocation);
            try
            {
                certStore.Open(OpenFlags.ReadOnly);
                foreach(var cert in certStore.Certificates)
                    if(cert.Subject == _certSubject)
                        return cert;

                throw new CertificateNotFoundException("Could not find certificate with the subject '" + _certSubject +
                    "' in the certificate store '" + _certLocation + "'");
            }
            finally
            {
                certStore.Close();
            }
        }

        public string Decrypt(string cyphertext)
        {
            var cert = _getCert();
            var sp = (RSACryptoServiceProvider)(cert.HasPrivateKey ? cert.PrivateKey : cert.PublicKey.Key);

            var input = Convert.FromBase64String(cyphertext);
            var output = sp.Decrypt(input, false);
            return Encoding.Default.GetString(output);
        }

        public string Encrypt(string plaintext)
        {
            var cert = _getCert();
            var sp = (RSACryptoServiceProvider)cert.PublicKey.Key;

            var input = Encoding.Default.GetBytes(plaintext);
            var output = sp.Encrypt(input, false);
            return Convert.ToBase64String(output);
        }
    }
}
