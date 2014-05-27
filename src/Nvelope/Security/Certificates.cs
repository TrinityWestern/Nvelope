using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace Nvelope.Security
{
    public static class Certificates
    {
        public static X509Certificate2 Find(string subject, StoreLocation location = StoreLocation.LocalMachine, 
            bool throwIfNotFound = false)
        {
            var res = Find(c => c.Subject == subject, location);
            if((res == null) && throwIfNotFound)
                throw new CertificateNotFoundException("Could not find certificate with the subject '" + subject +
                        "' in the certificate store '" + location + "'");

            return res;
        }

        public static X509Certificate2 Find(Func<X509Certificate2, bool> filter, StoreLocation location = StoreLocation.LocalMachine,
            bool throwIfNotFound = false)
        {
            var certStore = new X509Store(location);
            try
            {
                certStore.Open(OpenFlags.ReadOnly);
                var res = certStore.Certificates.ToList()
                    .FirstOr(filter, null);
                    
                if((res == null) && throwIfNotFound)
                    throw new CertificateNotFoundException("Could not find certificate that matched the filter " +
                        "in the certificate store '" + location + "'");

                return res;
            }
            finally
            {
                certStore.Close();
            }
        }

        /// <summary>
        /// Returns the first user's certificate - there generally seems to be at least one.
        /// This is helpful for when you just need a cert - ie, for testing
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 UserCert()
        {
            // Only find certificates that verify, otherwise you end up with a 
            // CryptographicException "Bad Key" when decrypting
            return Find(c => c.Verify(), StoreLocation.CurrentUser, false);
        }

        /// <summary>
        /// Load a certificate from a pfx file
        /// </summary>
        /// <param name="pfxFile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 FromPFXFile(string pfxFile, string password)
        {
            X509Certificate2 certificate = new X509Certificate2(pfxFile, password);
            return certificate;
        }
    }

    public static class CertificateCollectionExtensions
    {
        public static List<X509Certificate2> ToList(this X509Certificate2Collection coll)
        {
            var res = new List<X509Certificate2>();
            foreach (var cert in coll)
                res.Add(cert);
            return res;
        }
    }
}
