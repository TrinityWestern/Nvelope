using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    class NameValueCollectionExtensionTests
    {
        public NameValueCollection nvc
        {
            get
            {
                var nvc = new NameValueCollection();
                nvc.Add("a", "1");
                nvc.Add("b", "2");
                nvc.Add("c", "3");
                nvc.Add("d", "4");
                return nvc;
            }
        }

        [Test]
        public void Test()
        {

            var exclude = new List<string>() { "c", "e" };

            // Test Exclude
            Assert.AreEqual("([a,1],[b,2],[c,3],[d,4])", nvc.Print());
            // Test Exclude
            Assert.AreEqual("([a,1],[b,2],[d,4])", nvc.Except(exclude).Print());
            exclude[0] = "d";
            Assert.AreEqual("([a,1],[b,2],[c,3])", nvc.Except(exclude.ToArray()).Print());
            // Test the key enumeration
            Assert.AreEqual("(a,b,c,d)", nvc.Keys.Enumerate().Print());
        }

        [Test]
        public void ToDictionaty()
        {
            var collection = nvc;
            collection.Add(null, null);

            //If we pass in a null key, it should be converted to empty string
            Assert.IsTrue(collection.ToDictionary().ContainsKey(""));
        }
    }
}
