using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Reflection;

namespace Nvelope.Tests.Reflection
{
    [TestFixture]
    public class ReflectionOnDictionaryTests
    {
        [Test]
        public void SetsExtraKeys()
        {
            var t = new TestObj();
            var dict = new { Val1 = 7, Val2 = "abc" }._AsDictionary();

            Assert.AreEqual("([Val1,7],[Val2,abc])", t._SetFrom(dict).Print());
        }

        [Test]
        public void FieldsGetsKeysAndProperties()
        {
            var t = new TestObj();
            Assert.AreEqual("(Val1)", t._Fields().Print());
            t.Val1 = 1;
            Assert.AreEqual("(Val1)", t._Fields().Print());
            t.Add("Val2", 2);
            Assert.AreEqual("(Val1,Val2)", t._Fields().Print());
        }
    }

    public class TestObj : Dictionary<string, object>
    {
        public int Val1
        {
            get { return this.Val("Val1", 0).ConvertTo<int>(); }
            set { this["Val1"] = value; }
        }
    }
}
