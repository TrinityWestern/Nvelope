using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Combinatorics;

namespace Nvelope.Tests.Combinatorics
{
    [TestFixture]
    public class CombinatoricsListExtensionTests
    {
        [Test]
        public void Combinations()
        {
            var list = "a".And("b").And("c");

            var res = list.Combinations(2);
            Assert.AreEqual("((a,b),(a,c),(b,c))", res.Print());
        }

        [Test]
        public void Permutations()
        {
            var list = "a".And("b").And("c");

            var res = list.Permutations(2);
            Assert.AreEqual("((a,b),(a,c),(b,a),(b,c),(c,a),(c,b))", res.Print());
        }
    }
}
