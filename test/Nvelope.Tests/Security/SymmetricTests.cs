using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope.Security;
using NUnit.Framework;

namespace Nvelope.Tests.Encryption
{   
    public abstract class SymmetricTests
    {
        public abstract ISymmetricCryptor GetCryptor();

        [Test(Description = "I'm not going to bother trying to check whether it gives the proper cyphertext for Rijndael or anything like that. " +
            "As long as it turns the plaintext into something else and changes it back again, I'll call it good")]
        public void DoesSomething()
        {
            var plaintext = "The quick brown fox jumped over the lazy dog.";

            var cryptor = GetCryptor();
            var cyphertext = cryptor.Encrypt(plaintext);

            Assert.AreNotEqual(plaintext, cyphertext);
            Assert.AreNotEqual("", cyphertext);

            var decrypted = cryptor.Decrypt(cyphertext);
            Assert.AreEqual(plaintext, decrypted);
        }

        [Test]
        public void HandlesEmptyInput()
        {
            var plaintext = "";
            var cryptor = GetCryptor();
            var cyphertext = cryptor.Encrypt(plaintext);
            Assert.AreNotEqual(plaintext, cyphertext);
            Assert.AreNotEqual("", cyphertext);

            var decrypted = cryptor.Decrypt(cyphertext);
            Assert.AreEqual(plaintext, decrypted);
        }
    }
}
