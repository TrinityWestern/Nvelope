using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    public class SDictionaryTests
    {
        [Test]
        public void Adding()
        {
            var d = dict;
            bool ran = false;
            d.ItemAdding += c => ran = true;
            d.Add(4, "four");
            Assert.IsTrue(ran);
            Assert.AreEqual("four", d[4]);
        }
        [Test]
        public void AddedWithPair()
        {
            var d = dict;
            bool ran = false;
            d.ItemAdded += c => ran = true;
            d.Add(new KeyValuePair<int, string>(4, "four"));
            Assert.IsTrue(ran);
            Assert.AreEqual("four", d[4]);
        }
        [Test]
        public void Removing()
        {
            var d = dict;
            bool ran = false;
            d.ItemRemoving += c => ran = true;
            d.Remove(1);
            Assert.IsTrue(ran);
            Assert.IsFalse(d.ContainsKey(1));
        }
        [Test]
        public void Removed()
        {
            var d = dict;
            bool ran = false;
            d.ItemRemoved += c => ran = true;
            d.Remove(1);
            Assert.IsTrue(ran);
            Assert.IsFalse(d.ContainsKey(1));
        }
        [Test]
        public void Setting()
        {
            var d = dict;
            bool ran = false;
            d.ItemSetting += c => ran = true;
            d[2] = "TWO";
            Assert.IsTrue(ran);
            Assert.AreEqual("TWO", d[2]);
            d.ItemSetting += c => false;
            d[2] = "TWOTWO";
            Assert.AreEqual("TWO", d[2]);
        }
        [Test]
        public void Set()
        {
            var d = dict;
            bool ran = false;
            d.ItemSet += c => ran = true;
            d[2] = "TWO";
            Assert.IsTrue(ran);
            Assert.AreEqual("TWO", d[2]);
            d.ItemSetting += c => false;
            d[2] = "TWOTWO";
            Assert.AreEqual("TWO", d[2]);
        }
        [Test]
        public void WithNull()
        {
            var d = nullDict;
            Assert.AreEqual("one", d[1]);
            Assert.AreEqual("null", d[42]);
        }
        
        [Test]
        public void KeyValues()
        {
            var d = nullDict;
            Assert.AreEqual("(1,2,3)", d.Keys.Print());
            Assert.AreEqual("(one,two,three)", d.Values.Print());
        }

        [Test]
        public void Clear()
        {
            var d = dict;
            Assert.AreEqual(3, d.Count);
            d.Clear();
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void UnRemoveable()
        {
            var d = dict;
            d.ItemRemoving += c => false;
            d.Remove(1);
            Assert.IsTrue(d.ContainsKey(1));
        }

        [Test]
        public void Remove()
        {
            var d = dict;
            d.Remove(new KeyValuePair<int, string>(1, "one"));
            Assert.AreEqual(2, d.Count);
            d.Remove(new KeyValuePair<int, string>(2, "error"));
            Assert.AreEqual(2, d.Count);
        }

        [Test]
        public void Readonly()
        {
            Assert.IsFalse(dict.IsReadOnly);
        }

        [Test]
        public void Cotains()
        {
            var kv = new KeyValuePair<int, string>(2, "two");
            Assert.IsTrue(dict.Contains(kv));
        }
        [Test]
        public void Enumerator()
        {
            var keys = dict.Select(kv => kv.Key);
            Assert.IsTrue(keys.Contains(1));
            Assert.AreEqual(keys, dict.Keys);
        }
        [Test]
        public void TryGet()
        {
            var d = dict;
            string s;
            d.TryGetValue(1, out s);
            Assert.AreEqual("one", s);
            d.TryGetValue(190, out s);
            Assert.AreEqual(null, s);
        }

        private static SDictionary<int, string> dict
        {
            get {
                var res = new SDictionary<int, string>();
                res[1] = "one";
                res[2] = "two";
                res[3] = "three";
                return res;
            }
        }

        private static SDictionary<int, string> nullDict
        {
            get {
                var res = SDictionary<int, string>.WithNull("null");
                res[1] = "one";
                res[2] = "two";
                res[3] = "three";
                return res;

            }
        }


    }
}
