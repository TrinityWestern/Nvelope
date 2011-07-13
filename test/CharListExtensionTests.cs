using System.Linq;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class CharListExtensionTests
    {
        [Test]
        public void Inc()
        {
            var res = 'a'.Inc('b').Take(7);
            Assert.AreEqual("(a,b,aa,ab,ba,bb,aaa)", res.Print());
        }

        [Test]
        public void Chars()
        {
            string s = "Aqueduct";
            char[] c = { 'A', 'q', 'u', 'e', 'd', 'u', 'c', 't' };
            Assert.AreEqual(c, s.Chars());
        }

        [Test]
        public void Read()
        {
            char[] c = { 'A', 'q', 'u', 'e', 'd', 'u', 'c', 't' };
            Assert.AreEqual("Aqueduct", c.Read());
            Assert.AreEqual("A", 'A'.Read());
        }
    }
}
