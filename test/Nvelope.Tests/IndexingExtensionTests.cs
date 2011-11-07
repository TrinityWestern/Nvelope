using System;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class IndexingExtensionTests
    {
        Func<string, int> GetLength = new Func<string, int>(
            (string s) => { return s.Length; });


        [Test]
        public void IndexFlat()
        {
            Assert.AreEqual("([1,a],[2,bb])", new string[] { "a", "bb" }.IndexFlat(GetLength).Print());
            Assert.AreEqual("()", new string[] { }.IndexFlat(GetLength).Print());
        }

        [Test]
        public void Index()
        {
            Assert.AreEqual("([1,(a,b)],[2,(aa)])", new string[] { "a", "aa", "b" }.Index(GetLength).Print());
            Assert.AreEqual("()", new string[] { }.Index(GetLength).Print());
        }

        [Test]
        public void Map()
        {
            Assert.AreEqual("([a,1],[b,1],[bb,2])", new string[] { "a", "b", "bb" }.MapIndex(GetLength).Print());
            Assert.AreEqual("()", new string[] { }.MapIndex(GetLength).Print());
        }
    }
}
