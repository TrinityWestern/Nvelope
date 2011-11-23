using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Encryption;

namespace Nvelope.Tests.Encryption
{
    [TestFixture]
    public class SymmetricPasswordTests : SymmetricTests
    {
        public override ISymmetric GetCryptor()
        {
            return new SymmetricPassword("abc123");
        }
    }
}
