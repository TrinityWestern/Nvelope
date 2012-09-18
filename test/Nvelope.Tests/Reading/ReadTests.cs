using System.Linq;
using NUnit.Framework;
using Nvelope.Reading;

namespace Nvelope.Tests.Reading
{
    [TestFixture]
    public class ReadTests
    {
        [Test]
        public void List()
        {
            var res = Read.List("(a,bb,ccc)");
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual("a", res.First());
            Assert.AreEqual("bb", res.Second());
            Assert.AreEqual("ccc", res.Third());            
        }

        [Test]
        public void ListOfInt()
        {
            var res = Read.List<int>("(1,2,3)");
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(1, res.First());
            Assert.AreEqual(2, res.Second());
            Assert.AreEqual(3, res.Third());
        }

        [Test]
        public void Dict()
        {
            var res = Read.Dict("([a,1],[bb,],[c\"cc,3,s])");
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual("a", res.Keys.First());
            Assert.AreEqual("1", res.Values.First());
            // Make sure it works with empty values
            Assert.AreEqual("bb", res.Keys.Second());
            Assert.AreEqual("", res.Values.Second());
            // Make sure extra commas are interpreted as part of the value, not the key
            Assert.AreEqual("c\"cc", res.Keys.Third());
            Assert.AreEqual("3,s", res.Values.Third());
        }

        [Test]
        public void DictOfStrToInt()
        {
            var res = Read.Dict<string, int>("([a,1],[bbb,2],[\"c\",3])");
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual("a", res.Keys.First());
            Assert.AreEqual(1, res.Values.First());
            Assert.AreEqual("bbb", res.Keys.Second());
            Assert.AreEqual(2, res.Values.Second());
            Assert.AreEqual("\"c\"", res.Keys.Third());
            Assert.AreEqual(3, res.Values.Third());
        }

        [Test]
        public void DictWithWhitespace()
        {
            var res = Read.Dict(@"([a,1],
                [bb,2],
                [ccc,3]");
            Assert.AreEqual("([a,1],[bb,2],[ccc,3])", res.Print());
        }

        [Test]
        public void NestedDicts()
        {
            var res = Read.Dict("([A,1],[B,([Id,1])])");
            Assert.AreEqual("1", res["A"]);
            Assert.AreEqual("([Id,1])", res["B"]);
            
            res = Read.Dict("([A,1],[B,([Id,1])],[C,3])");
            Assert.AreEqual("1", res["A"]);
            Assert.AreEqual("([Id,1])", res["B"]);
            Assert.AreEqual("3", res["C"]);

            res = Read.Dict("([A,1],[B,([Id,1],[Name,test])],[C,3])");
            Assert.AreEqual("1", res["A"]);
            Assert.AreEqual("([Id,1],[Name,test])", res["B"]);
            Assert.AreEqual("3", res["C"]);
        }

        [Test]
        public void DoesntBreakOnBadNestedDicts()
        {
            var res = Read.Dict("([A,1],[B,([Id,1])");
            Assert.AreEqual("1", res["A"]);
            Assert.AreEqual("([Id,1", res["B"]);
        }
    }
}
