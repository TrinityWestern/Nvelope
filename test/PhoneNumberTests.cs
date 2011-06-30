using System;
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

            number = new PhoneNumber("1-(878)-443-2908x23");
            Assert.AreEqual("1", number.Country);
            Assert.AreEqual("878", number.Area);
            Assert.AreEqual("443-2908", number.Local);
            Assert.AreEqual("23", number.Extension);

            // This is an example phone number from China
            number = new PhoneNumber("(0755) 1234 5878");
            Assert.AreEqual("0755", number.Area);
            Assert.AreEqual("1234 5878", number.Local);
            // A phone number from the Netherlands
            number = new PhoneNumber("+ 31-77-4656767");
            Assert.AreEqual("31", number.Country);
            Assert.AreEqual("77", number.Area);
            Assert.AreEqual("4656767", number.Local);
            // testing robustness
            number = new PhoneNumber("+ 31-774656767");
            Assert.AreEqual("31", number.Country);
            Assert.AreEqual("", number.Area);
            Assert.AreEqual("774656767", number.Local);
            // a number I found in our data
            number = new PhoneNumber("(604)7627111");
            Assert.AreEqual("", number.Country);
            Assert.AreEqual("604", number.Area);
            Assert.AreEqual("7627111", number.Local);
        }
        
        [Test,Ignore]
        public void BrokenNumberToFix()
        {
            // a number I found in our data
            // TOOD: this is currently broken
            var number = new PhoneNumber("604 783 8072");
            Assert.AreEqual("", number.Country);
            Assert.AreEqual("604", number.Area);
            Assert.AreEqual("783 8072", number.Local);
        }


        [Test]
        public void InvalidNumbers()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhoneNumber(""));
        }
        [Test]
        public void CreateAnyway()
        {
            const string number = "002 - 379 985-";
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhoneNumber(number));
            var phone = PhoneNumber.CreateAnyway(number);
            Assert.AreEqual("", phone.Country);
            Assert.AreEqual("", phone.Area);
            Assert.AreEqual("002 - 379 985-", phone.Local);
            Assert.AreEqual("", phone.Extension);
            
        }

        [Test]
        public void Print()
        {
            var number = new PhoneNumber("1-878-443-2908 ext. 23");
            Assert.AreEqual("1-878-443-2908x23", number.ToString());

            number = new PhoneNumber("6048537994");
            Assert.AreEqual("6048537994", number.ToString());

            number = new PhoneNumber("-604-5527455x");
            Assert.AreEqual("604-5527455", number.ToString());
            // test for manually set values
            number = new PhoneNumber {
                Country = null,
                Area = null,
                Local = "12346",
                Extension = null
            };

            Assert.AreEqual("12346", number.ToString());
        }
    }
}
