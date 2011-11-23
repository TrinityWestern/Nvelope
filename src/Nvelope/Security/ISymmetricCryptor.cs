using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Security
{
    /// <summary>
    /// Defines symmetric (2-way) encryption
    /// </summary>
    public interface ISymmetricCryptor
    {
        string Encrypt(string plaintext);
        string Decrypt(string cyphertext);
    }
}
