using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Nvelope.Encryption
{
    /// <summary>
    /// Implements symmetric encryption using a password.
    /// </summary>
    public class SymmetricPassword : ISymmetric
    {
        public SymmetricPassword(string password)
        {
            _password = password;
            _salt = Encoding.Default.GetBytes("Rfc2898DeriveBytes needs the salt to always be constant" +
                " in order to generate the same key and initial vector. However, we do want it to be " +
                "disinct on password... so: " + password);
        }

        private string _password;
        private byte[] _salt;

        private SymmetricAlgorithm _getAlgo()
        {
            var res = Rijndael.Create();
            res.Padding = PaddingMode.ISO10126;
            return res;
        }

        public virtual string Encrypt(string plaintext)
        {
            using (var algo = _getAlgo())
            {   
                var input = Encoding.Default.GetBytes(plaintext);

                var config = new Rfc2898DeriveBytes(_password, _salt);
                var key = config.GetBytes(32);
                var iv = config.GetBytes(16);

                var cryptor = algo.CreateEncryptor(key, iv);

                var output = cryptor.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output);
            }
        }

        public virtual string Decrypt(string cyphertext)
        {
            using (var algo = _getAlgo())
            {
                var input = Convert.FromBase64String(cyphertext);

                var config = new Rfc2898DeriveBytes(_password, _salt);
                var key = config.GetBytes(32);
                var iv = config.GetBytes(16);

                var cryptor = algo.CreateDecryptor(key, iv);

                var output = cryptor.TransformFinalBlock(input, 0, input.Length);
                return Encoding.Default.GetString(output);
            }
        }
    }
}
