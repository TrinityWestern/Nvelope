using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    public class FixedSizeQueueTests
    {
        [Test]
        public void Works()
        {
            var q = new FixedSizeQueue<int>(2);
            q.Enqueue(1);
            Assert.AreEqual("(1)", q.Print());
            q.Enqueue(2);
            Assert.AreEqual("(1,2)", q.Print());
            q.Enqueue(3);
            Assert.AreEqual("(2,3)", q.Print());
        }
    }
}
