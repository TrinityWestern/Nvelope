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
        public void NumTimesCharacterRepeatsAtEndTest()
        {
            char quote = '\'';
            Assert.AreEqual(1, "Kent'".NumTimesCharacterRepeatsAtEnd(quote));
            Assert.AreEqual(2, "Kent\'\'".NumTimesCharacterRepeatsAtEnd(quote));
            Assert.AreEqual(0, "kent' ".NumTimesCharacterRepeatsAtEnd(quote));
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
        public void RetrieveIntInString()
        {
            string s = "This is a string with 10 in it";
            string t = "This is a string with no numbers in it";
            int intInString = StringExtensions.RetrieveIntInString(s); 
            int noIntInString = StringExtensions.RetrieveIntInString(t);
            Assert.AreEqual(10, intInString);
            Assert.AreEqual(0, noIntInString);
        }

        [Test]
        public void FindIntInString()
        {
            string s = "This is a string with 10 in it";
            string t = "This is a string with no numbers in it";
            string intInString = StringExtensions.FindIntInString(s);
            string noIntInString = StringExtensions.FindIntInString(t);
            Assert.AreEqual("10", intInString);
            Assert.AreEqual("0", noIntInString);
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
        public void GetEmailInParanthesis()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            string t = "Joe Sward (joe.sward@twu.ca)";
            Assert.AreEqual(null, s.GetEmailInParenthesis());
            Assert.AreEqual("joe.sward@twu.ca", t.GetEmailInParenthesis());
        }

        [Test]
        public void GetIntegerInParenthesis()
        {
            string s = "The quick brown fox jumped over the lazy dog.";
            string t = "Joe Sward (93237)";
            Assert.AreEqual(null, s.GetIntegerInParenthesis());
            Assert.AreEqual(93237, t.GetIntegerInParenthesis());
        }

        [Test]
        public void ToDoubleAsStringOr0()
        {
            string s = "Some value = 1.35";
            string t = "1.35";
            Assert.AreEqual("0", StringExtensions.ToDoubleAsStringOr0(s));
            Assert.AreEqual("1.35", StringExtensions.ToDoubleAsStringOr0(t));
        }

        [Test]
        public void ToIntOr0()
        {
            string s = "Some value = 135";
            string t = "135";
            Assert.AreEqual(0, StringExtensions.ToIntOr0(s));
            Assert.AreEqual(135, StringExtensions.ToIntOr0(t));
        }

        [Test]
        public void SplitPair()
        {
            string s = "this is a string, separated by a comma";
            string[] sout = { "this is a string", " separated by a comma" };
            Assert.AreEqual(sout, s.SplitPair(","));
        }

        [Test]
        public void SplitPairException()
        {
            string s = "this is a string, separated by a comma, with a third part thrown in";
            var ex = Assert.Throws<Exception>(
      () => s.SplitPair(","));
            // this is a really annoying and not very helpful message
            // but that's what we get for using string.Format
            Assert.AreEqual("The pair was not specified correctly. A pair can only be two items and must be separated by a specified string. In this case, the specified string was ','.", ex.Message);
        }

        [Test]
        public void Split()
        {
            string s = "this is a string, separated by a comma, with a third part thrown in";
            string[] sout = { "this is a string", " separated by a comma", " with a third part thrown in" };
            Assert.AreEqual(sout, s.Split(","));
        }

        [Test]
        public void SemiClean()
        {
            string s = "<note> <to>Tove</to><from>Jani, \"Tina\"</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>";
            Assert.AreEqual("note toTove/tofromJani \"\"Tina\"\"/fromheadingReminder/headingbodyDon''t forget me this weekend!/body/note", StringExtensions.SemiClean(s));
        }
    }
}
       
