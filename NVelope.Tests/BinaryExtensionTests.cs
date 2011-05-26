using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class BinaryExtensionTests
    {
        [Test]
        public void ToHexString()
        {
            var bytes = new byte[] { Byte.MaxValue, Byte.MaxValue, Byte.MaxValue };
            Assert.AreEqual("FFFFFF", bytes.ToHexString());
            bytes = new byte[] { Byte.MinValue, Byte.MinValue, Byte.MinValue };
            Assert.AreEqual("000000", bytes.ToHexString());
            bytes = new byte[] { 0x1F, 0x2D, 0xF7 };
            Assert.AreEqual("1F2DF7", bytes.ToHexString());
        }

        [Test]
        public void ToUTF8String()
        {
            var bytes = new byte[] { 0x31, 0x36, 0x38, 0x33, 0x33, 0x37, 0x3A, 0x42, 0x72, 0x69, 0x6F, 0x6E };
            Assert.AreEqual("168337:Brion", bytes.ToUTF8String());
        }
    }
}
