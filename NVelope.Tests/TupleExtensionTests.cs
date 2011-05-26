using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class TupleExtensionTests
    {

        [Test]
        public void FirstCol()
        {
            var list = Tuple.Create(1, 2).And(Tuple.Create(3, 4)).And(Tuple.Create(5, 6));
            Assert.AreEqual("(1,3,5)", list.FirstCol().Print());

            var l2 = Tuple.Create(1, 2, 'b', 'a').And(Tuple.Create(3, 4, 'a', 'c')).And(Tuple.Create(5, 6, 'e', 'q'));
            Assert.AreEqual("(1,3,5)", l2.FirstCol().Print());

            var l3 = Tuple.Create('x', '_', 'o')
                .And(Tuple.Create('x', 'o', '_'))
                .And(Tuple.Create('x', '_', 'o'));
            Assert.AreEqual("(x,x,x)", l3.FirstCol().Print());
        }
    }
}
