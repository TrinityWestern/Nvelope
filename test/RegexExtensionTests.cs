using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope;
using System.Text.RegularExpressions;

namespace Nvelope.Tests
{
    [TestFixture]
    public class RegexExtensionTests
    {
        [Test]
        public void ToList()
        {
            var match = Regex.Match("abcd", ".(b).(d)");
            var groups = match.Groups.ToList();
            Assert.AreEqual(3, groups.Count());
            Assert.AreEqual("abcd", groups.First().Value);
            Assert.AreEqual("b", groups.Second().Value);
            Assert.AreEqual("d", groups.Third().Value);
        }

        [Test]
        public void Print()
        {
            var match = Regex.Match("abcd", ".(b).(d)");
            Assert.AreEqual("(abcd,b,d)", match.Print());
        }
    }
}
