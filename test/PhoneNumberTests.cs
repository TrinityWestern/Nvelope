using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    class PhoneNumberTests
    {
        [Test]
        public void ValidNumbers()
        {
            var number = new PhoneNumber("7788782908");
            Assert.AreEqual("7788782908", number.Local);

            number = new PhoneNumber("778-878-2908");
            Assert.AreEqual("878-2908", number.Local);

            // This seems kinda wierd :-/
            number = new PhoneNumber("878-2908");
            Assert.AreEqual("878-2908", number.Local);

            number = new PhoneNumber("1-878-443-2908 ext. 23");
            Assert.AreEqual("1", number.Country);
            Assert.AreEqual("878", number.Area);
            Assert.AreEqual("443-2908", number.Local);
            Assert.AreEqual("23", number.Extension);

            number = new PhoneNumber("1-878-443-2908x23");
            Assert.AreEqual("1", number.Country);
            Assert.AreEqual("878", number.Area);
            Assert.AreEqual("443-2908", number.Local);
            Assert.AreEqual("23", number.Extension);
        }

        [Test]
        public void InValidNumbers()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhoneNumber(""));
        }

        [Test]
        public void Print()
        {
            var number = new PhoneNumber("1-878-443-2908 ext. 23");
            Assert.AreEqual("1-878-443-2908x23", number.ToString());
        }
    }
}
