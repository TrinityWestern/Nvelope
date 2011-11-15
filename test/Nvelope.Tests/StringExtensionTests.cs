using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
        }

        [Test]
        public void RemoveEndTest()
        {
            var str = "kent";
            Assert.AreEqual("ken", str.RemoveEnd(1));
            Assert.AreEqual("ke", str.RemoveEnd(2));
            Assert.AreEqual("", str.RemoveEnd(100));
        }

        [Test]
        public void Join()
        {
            Assert.AreEqual("a,b,cc", new string[] { "a", "b", "cc" }.Join(","));
            Assert.AreEqual("aa and bb and cc", new string[] { "aa", "bb", "cc" }.Join(" and "));
            Assert.AreEqual("a", new string[] { "a" }.Join(","));
            Assert.AreEqual("", new string[] { }.Join("   "));

        }

        [Test]
        public void ContainsAny()
        {
            Assert.IsTrue("abcdef".ContainsAny("ab", "cd", "ef"));
            Assert.IsTrue("abcdef".ContainsAny("ab", "cd", "xy"));
            Assert.IsFalse("abcdef".ContainsAny("qw", "er", "ty"));
        }

        [Test]
        public void ContainsAll()
        {
            Assert.IsTrue("abcdef".ContainsAll("ab", "cd", "abcd"));
            Assert.IsFalse("abcdef".ContainsAll("ab", "efgh"));            
        }

        [Test]
        public void ChopStart()
        {
            Assert.AreEqual("def", "abcdef".ChopStart("abc"));
            Assert.AreEqual("def", "abcdef".ChopStart("a", "b", "c"));
        }

        [Test]
        public void ChopEnd()
        {
            Assert.AreEqual("abc", "abcdef".ChopEnd("def"));
            Assert.AreEqual("abc", "abcdef".ChopEnd("d", "e", "f"));
        }

        [Test]
        public void DictFormat()
        {
            var dict = new Dictionary<string, string>();
            dict["h"] = "hello";
            dict["w"] = "world";
            dict["f"] = "foo";
            var result = "{h} {w}".FormatFromDictionary(dict);
            Assert.AreEqual("hello world", result);
        }
        [Test]
        public void DictFormatException()
        {
            var dict = new Dictionary<string, string>();
            dict["h"] = "hello";
            dict["w"] = "world";
            var ex = Assert.Throws<FormatException>(
                () => "{h} {notcontained}".FormatFromDictionary(dict));
            // this is a really annoying and not very helpful message
            // but that's what we get for using string.Format
            Assert.AreEqual("Input string was not in a correct format.", ex.Message);
        }

        [Test]
        public void ToLength()
        {
            Assert.AreEqual("abc   ", "abc".ToLength(6));
            Assert.AreEqual("abc", "abc".ToLength(3));
            Assert.AreEqual("abc", "abcd".ToLength(3));
            Assert.AreEqual("", "abcd".ToLength(0));
            string s = null;
            Assert.AreEqual("  ", s.ToLength(2));
        }

        [Test]
        public void Repeat()
        {
            Assert.AreEqual("abcabc", "abc".Repeat(2));
            Assert.AreEqual("abcabcabc", "abc".Repeat(3));
            Assert.AreEqual("aaaaa", "a".Repeat(5));
        }

        [Test]
        public void Tokenize()
        {

            IEnumerable<string> tokenizeTestString = "The quick brown fox jumped over the lazy dog.".Tokenize(@"^\s*([A-Za-z0-9]+)");
            Assert.AreEqual("(The,quick,brown,fox,jumped,over,the,lazy,dog,.)", tokenizeTestString.Print());
                   
            IEnumerable<string> tokenizeTest =  "/A/B/C/D".Tokenize("^/([^/]*)");
            Assert.AreEqual("(A,B,C,D)", tokenizeTest.Print());
        }

        [Test]
        public void TokenizeDefaultsToSplittingOnWhitespace()
        {
            var res = "a and bee".Tokenize();
            Assert.AreEqual("(a,and,bee)", res.Print());
        }

        [Test]
        public void IsNullOrEmpty()
        {
            string nullString = null;
            string emptyString = "";
            Assert.AreEqual(true, nullString.IsNullOrEmpty());
            Assert.AreEqual(true, emptyString.IsNullOrEmpty());
            Assert.AreNotEqual(true, "This is a string".IsNullOrEmpty());
        }

        [Test]
        public void ToStringN()
        {
            string nullString = null;
            string someString = "This is a string";
            Assert.AreEqual("", nullString.ToStringN());
            Assert.AreEqual(someString, someString.ToStringN());
        }

        [Test]
        public void TrimN()
        {
            string nullString = null;
            string someString = " This is a string. ";
            Assert.AreEqual(null, nullString.TrimN());
            Assert.AreEqual("This is a string.", someString.TrimN());
        }

        [Test]
        public void SubstringBefore()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            Assert.AreEqual("The quick brown fox ", s.SubstringBefore("jumped"));
            Assert.AreEqual("The", s.SubstringBefore(" "));
        }

        [Test]
        public void SubstringAfter()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            Assert.AreEqual(" over the lazy dog.", s.SubstringAfter("jumped"));
            Assert.AreEqual("quick brown fox jumped over the lazy dog.", s.SubstringAfter(" "));
        }

        [Test]
        public void UnCamelize()
        {
            string s = "camelCaseStringTest1";
            string t = "CamelCaseStringTest2";
            Assert.AreEqual("camel Case String Test1", s.UnCamelize());
            Assert.AreEqual("camel case string test2", t.UnCamelize(true));
        }

        [Test]
        public void Camelize()
        {
            string s = "the quick brown fox jumped over the lazy dog.";
            Assert.AreEqual("theQuickBrownFoxJumpedOverTheLazyDog.", s.Camelize());
            Assert.AreEqual("TheQuickBrownFoxJumpedOverTheLazyDog.", s.Camelize(true));
        }

        [Test]
        public void Excerpt()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            Assert.AreEqual("The quick brown ...", s.Excerpt(20));
        }

        [Test]
        public void Strip()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            string[] t = { " ", "." };
            string[] u = { "o" };
            string[] v = { "jumped", "dog" };
            Assert.AreEqual("Thequickbrownfoxjumpedoverthelazydog", s.Strip(t));
            Assert.AreEqual("The quick brwn fx jumped ver the lazy dg.", s.Strip(u));
            Assert.AreEqual("The quick brown fox  over the lazy .", s.Strip(v));
        }


        [Test]
        public void Split()
        {
            string s = "this is a string, separated by a comma, with a third part thrown in";
            string[] sout = { "this is a string", " separated by a comma", " with a third part thrown in" };
            Assert.AreEqual(sout, s.Split(","));
        }
    }
}
       
