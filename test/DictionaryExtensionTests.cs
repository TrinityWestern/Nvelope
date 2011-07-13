using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class DictionaryExtensionTests
    {
        [Test]
        public void Realize()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("a", "a");
            dict.Add("b", new Func<object>(() => "b"));

            // Realize should recognize that the value of b
            // is a function, and it should evaluate that function
            // and return the result
            Assert.AreEqual("([a,a],[b,b])", dict.Realize().Print());
        }

        [Test]
        public void Assoc()
        {
            var dict = new Dictionary<string, int>();
            dict.Add("a", 1);
            var res = dict.Assoc("b", 2);
            Assert.AreEqual("([a,1],[b,2])", res.Print());
        }

        [Test]
        public void Ensure()
        {
            var dict = new Dictionary<string, int>();
            dict.Ensure("a", 1);
            Assert.AreEqual("([a,1])", dict.Print());
            dict.Ensure("a", 2);
            Assert.AreEqual("([a,2])", dict.Print());
        }

        [Test]
        public void Val()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("1", "a");

            Assert.AreEqual("a", dict.Val("1", "z"));
            Assert.AreEqual("z", dict.Val("2", "z"));

            Assert.AreEqual("a", dict.Val("1"));
            Assert.AreEqual(null, dict.Val("2"));            
        }

        [Test]
        public void SetVal()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "1");

            dict.SetVal(2, "2");
            Assert.AreEqual("([1,1],[2,2])", dict.Print());
            dict.SetVal(1, "3");
            Assert.AreEqual("([1,3],[2,2])", dict.Print());
        }


        [Test]
        public void Except()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            Assert.AreEqual("([1,one],[2,two])", dict.Except(3).Print());
            Assert.AreEqual("([2,two])", dict.Except(1, 3).Print());
            Assert.AreEqual("([1,one],[2,two],[3,three])", dict.Except(57).Print());
        }

        [Test]
        public void Only()
        {
            var dict = new Dictionary<int, string>();
            dict.Add(1, "one");
            dict.Add(2, "two");
            dict.Add(3, "three");

            Assert.AreEqual("()", dict.Only(57).Print());
            Assert.AreEqual("([1,one])", dict.Only(1).Print());
            Assert.AreEqual("([1,one],[3,three])", dict.Only(1,3).Print());
        }

        [Test]
        public void WhereKeys()
        {
            var dict = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, { 3, "three" } };

            Assert.AreEqual("([1,one],[2,two])", dict.WhereKeys(k => k < 3).Print());
        }

        [Test]
        public void WhereValues()
        {
            var dict = new Dictionary<string, int>() { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            Assert.AreEqual("([one,1],[two,2])", dict.WhereValues(v => v < 3).Print());
        }

        [Test]
        public void Copy()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Key 1", "Value 1");
            dict.Add("Key 2", "Value 2");
            dict.Add("Key 3", "Value 3");
            var dictCopy = dict.Copy();
            Assert.AreEqual(dictCopy, dict);
        }

        [Test]
        public void ReadType()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("Key 1", "Value 1");
            dict.Add("Key 2", "Value 2");
            dict.Add("Key 3", "Value 3");
            var t = dict.Read<List>();
            Assert.AreEqual(typeof(NUnit.Framework.List), t.GetType());
        }

        [Test]
        public void AddRangeTest()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Key 1", "Value 1");
            dict.Add("Key 2", "Value 2");
            dict.Add("Key 3", "Value 3");
            IEnumerable<KeyValuePair<string, string>> range = kvp("Key 4", "Value 4");
            dict.AddRange(range);
            Dictionary<string, string> dictCopy = new Dictionary<string, string>();
            dictCopy.Add("Key 1", "Value 1");
            dictCopy.Add("Key 2", "Value 2");
            dictCopy.Add("Key 3", "Value 3");
            dictCopy.Add("Key 4", "Value 4");
            Assert.AreEqual(dictCopy, dict);
        }

        [Test]
        public void SelectTest()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Key 1", "Value 1");
            dict.Add("Key 2", "Value 2");
            dict.Add("Key 3", "Value 3");
            Dictionary<string, object> dictCopy = new Dictionary<string, object>();
            dictCopy.Add("Key 1", "Value 1");
            dictCopy.Add("Key 2", "Value 2");
            dictCopy.Add("Key 3", "Value 3");
            Assert.AreEqual(dictCopy, dict.Select(k => k, v => v as object));
        }

        [Test]
        public void SelectValsTest()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("Key 1", 1);
            dict.Add("Key 2", 2);
            dict.Add("Key 3", 3);
            Dictionary<string, decimal> dictCopy = new Dictionary<string, decimal>();
            dictCopy.Add("Key 1", 1);
            dictCopy.Add("Key 2", 2);
            dictCopy.Add("Key 3", 3);
            Assert.AreEqual(dictCopy, dict.SelectVals(v => v.ConvertTo<decimal>()));
        }

        [Test]
        public void SelectKeysTest()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Add(3, 3);
            Dictionary<decimal, int> dictCopy = new Dictionary<decimal, int>();
            dictCopy.Add(1, 1);
            dictCopy.Add(2, 2);
            dictCopy.Add(3, 3);
            Assert.AreEqual(dictCopy, dict.SelectKeys(k => k.ConvertTo<decimal>()));
        }

        [Test]
        public void Update()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Add(3, 3);
            dict.Add(4, 2);
            Dictionary<int, int> dictCopy = new Dictionary<int, int>();
            dictCopy.Add(1, 3);
            dictCopy.Add(3, 9);
            dict.Update(dictCopy);
            Dictionary<int, int> dictOther = new Dictionary<int, int>();
            dictOther.Add(1, 3);
            dictOther.Add(2, 2);
            dictOther.Add(3, 9);
            dictOther.Add(4, 2);
            Assert.AreEqual(dictOther, dict);
        }

        [Test]
        public void RemoveWhereValue()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Add(3, 3);
            dict.Add(4, 2);
            Dictionary<int, int> dictCopy = new Dictionary<int, int>();
            dictCopy.Add(1, 1);
            dictCopy.Add(3, 3);
            Assert.AreEqual(dictCopy, dict.RemoveWhereValue(v => v == 2));
        }

        [Test]
        public void RemoveWhereKey()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Add(3, 3);
            Dictionary<int, int> dictCopy = new Dictionary<int, int>();
            dictCopy.Add(1, 1);
            dictCopy.Add(3, 3);
            Assert.AreEqual(dictCopy, dict.RemoveWhereKey(k => k == 2));
        }

        [Test]
        public void IsSameAs()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Add(3, 3);
            Dictionary<int, int> dictCopy = new Dictionary<int, int>();
            dictCopy.Add(1, 1);
            dictCopy.Add(2, 2);
            dictCopy.Add(3, 3);
            Dictionary<int, int> dictOther = new Dictionary<int, int>();
            dictOther.Add(1, 3);
            dictOther.Add(7, 2);
            dictOther.Add(3, 9);
            Assert.AreEqual(true, dict.IsSameAs(dictCopy));
            Assert.AreEqual(false, dict.IsSameAs(dictOther));
        }

        [Test]
        public void IsSameAsParams()
        {
            var dict = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 2 }, { "Key 3", 3 } };
            Dictionary<string, int> dictCopy = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 5 }, { "Key 3", 3 } };
            string[] keysToCheck = { "Key 1", "Key 3" };
            Assert.AreEqual(false, dict.IsSameAs(dictCopy));
            Assert.AreEqual(true, dict.IsSameAs(dictCopy, keysToCheck));
        }

        [Test]
        public void ValuesForKeysParams()
        {
            var dict = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 2 }, { "Key 3", 3 } };
            string[] keysToCheck = { "Key 1", "Key 3" };
            IEnumerable<int> vals = dict.ValuesForKeys(keysToCheck);
            int x = 1;
            foreach (int v in vals)
            {
                Assert.AreEqual(x, v);
                x = x + 2;
            }

        }

         [Test]
        public void IsOneToOne()
        {
            var dict = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 2 }, { "Key 3", 3 } };
            var dictCopy = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 1 }, { "Key 3", 3 } };
            Assert.AreEqual(true, dict.IsOneToOne());
            Assert.AreEqual(false, dictCopy.IsOneToOne());
        }

        [Test]
        public void Invert()
        {
            var dict = new Dictionary<string, int>()
                { { "Key 1", 1 }, { "Key 2", 2 }, { "Key 3", 3 } };
            var dictCopy = new Dictionary<int, string>()
                { { 1, "Key 1" }, { 2, "Key 2" }, { 3, "Key 3" } };
            Assert.AreEqual(dictCopy, dict.Invert());
        }

        [Test]
        public void Union()
        {
            var dict = new Dictionary<int, int>()
                { { 1, 1 }, { 2, 2 }, { 3, 3 } };
            var dictCopy = new Dictionary<int, int>()
                { { 1, 17 }, { 4, 4 }, { 5, 5 } };
            var dictOther = new Dictionary<int, int>()
                { { 1, 17 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 } };
            Assert.AreEqual(dictOther, dict.Union(dictCopy));
        }

        [Test]
        public void Each()
        {
            var dict = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var res = "";
            dict.Each((i, s) => res += i + s + ",");
            Assert.AreEqual("1one,2two,", res);
        }

        [Test]
        public void KeysToSet()
        {
            var dict = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" } };
            var ints = new HashSet<int> { 1, 2 };
            Assert.IsTrue(dict.Keys.ToSet().SetEquals(ints));
        }

        private IEnumerable<KeyValuePair<string, string>> kvp(string key, string value)
        {
            yield return new KeyValuePair<string, string>(key, value);
        }
    }
}
