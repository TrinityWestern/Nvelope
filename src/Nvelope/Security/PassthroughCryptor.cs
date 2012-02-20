using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Security
{
    /// <summary>
    /// Implements "encryption" by doing nothing.
    /// This class is handy as a dummy implementation of the ISymmetricCryptor
    /// </summary>
    public class PassthroughCryptor : ISymmetricCryptor
    {
        public string Encrypt(string plaintext)
        {
            return plaintext;
        }

        public string Decrypt(string cyphertext)
        {
            return cyphertext;
        }
    }
}
