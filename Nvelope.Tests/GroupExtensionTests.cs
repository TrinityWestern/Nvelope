using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class GroupExtensionTests
    {

        [Test]
        public void HavingMax()
        {
            var list = new string[] { "aa", "a", "b", "bbb", "bbb" };
            Assert.AreEqual("(aa,bbb)", list.GroupBy(s => s.First()).HavingMax(s => s.Length).Print());
        }

        [Test]
        public void HavingMin()
        {
            var list = new string[] { "aa", "a", "b", "bbb", "bbb" };
            Assert.AreEqual("(a,b)", list.GroupBy(s => s.First()).HavingMin(s => s.Length).Print());
        }

        [Test]
        public void HavingDistinct()
        {
            var list = new string[] { "aa", "a", "b", "bbb", "bbb" };
            Assert.AreEqual("(aa,a,bbb)", list.HavingDistinct(s => s.Length).Print());
        }
    }
}
