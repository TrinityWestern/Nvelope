using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    public class DictionaryMirrorTests
    {
        [Test]
        public void AddEtc()
        {
            var m = new DictionaryMirror<int, string>();
            m.Add(1, "one");
            m.Add(2, "two");

            Assert.AreEqual(2, m["two"]);
            Assert.AreEqual("two", m[2]);
            Assert.AreEqual("(1,2)", m.AllFirst().Print());
            Assert.AreEqual("(one,two)", m.AllSecond().Print());
        }
    }
}
