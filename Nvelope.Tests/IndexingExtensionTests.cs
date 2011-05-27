using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class IndexingExtensionTests
    {
        [Test]
        public void IndexFlat()
        {
            Assert.AreEqual("([1,a],[2,bb])", new string[] { "a", "bb" }.IndexFlat(s => s.Length).Print());
            Assert.AreEqual("()", new string[] { }.IndexFlat(s => s).Print());
        }

        [Test]
        public void Index()
        {
            Assert.AreEqual("([1,(a,b)],[2,(aa)])", new string[] { "a", "aa", "b" }.Index(s => s.Length).Print());
            Assert.AreEqual("()", new string[] { }.Index(s => s).Print());
        }

        [Test]
        public void Map()
        {
            Assert.AreEqual("([a,1],[b,1],[bb,2])", new string[] { "a", "b", "bb" }.MapIndex(s => s.Length).Print());
            Assert.AreEqual("()", new string[] { }.MapIndex(s => s.Length).Print());
        }
    }
}
