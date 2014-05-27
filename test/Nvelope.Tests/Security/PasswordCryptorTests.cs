using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Security;

namespace Nvelope.Tests.Encryption
{
    [TestFixture]
    public class PasswordCryptorTests : SymmetricTests
    {
        public override ISymmetricCryptor GetCryptor()
        {
            return new PasswordCryptor("abc123");
        }
    }
}
