using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope;
using Nvelope.Reflection;

namespace Nvelope.Tests
{
    [TestFixture]
    public class DictionaryListExtensionTests
    {
        [Test]
        public void Select()
        {
            var d = new { A = 1, B = "2" }._AsDictionary();
            var dicts = d.And(d.Assoc("B", "42"));

            Assert.AreEqual("(2,42)", dicts.Select("B").Print());
        }

        [Test]
        public void Distinct()
        {
            var d = new { A = 1, B = "2" }._AsDictionary();
            var dicts = d.And(d.Assoc("B", "42"));

            Assert.AreEqual("(2,42)", dicts.Distinct("B").Print());
            Assert.AreEqual("(1)", dicts.Distinct("A").Print());
        }
    }
}
