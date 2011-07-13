using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    class RangeTests
    {
        protected Range<OrderedInteger> ones;
        protected Range<OrderedInteger> one_zero;
        protected Range<OrderedInteger> many;

        [TestFixtureSetUp]
        public void Setup()
        {
            ones = MakeRange(1, 1);
            one_zero = MakeRange(1, 0);
            many = MakeRange(1, 100);
        }
        [Test]
        public void RangeCount()
        {
            Assert.AreEqual(100, many.Count);
            Assert.AreEqual(1, ones.Count);
            Assert.AreEqual(2, one_zero.Count);
        }
        [Test]
        public void RangeToString()
        {
            Assert.AreEqual("1—0", MakeRange(1, 0).ToString());
            // I'm not sure what the most desireable behavior is
            // in this case, but this is how it works currently
            Assert.AreEqual("1—1", MakeRange(1, 1).ToString());
        }
        [Test]
        public void RangeContains()
        {
            var one = new OrderedInteger(1);
            var fortynine = new OrderedInteger(49);

            Assert.IsTrue(one_zero.Contains(one));
            Assert.IsTrue(many.Contains(fortynine));
            Assert.IsTrue(ones.Contains(one));
            Assert.IsFalse(ones.Contains(fortynine));
        }
        [Test]
        public void RangeFlip()
        {
            Assert.IsTrue(many.Count == many.Flip().Count);
        }
        [Test]
        public void RangeGetEnumerator()
        {
            var two = new OrderedInteger(2);
            var nintynine = new OrderedInteger(99);

            Assert.AreEqual(two, many.Second());
            Assert.AreEqual(nintynine, many.Flip().Second());

            var list = new List<object>();
            foreach (var i in many)
                list.Add(i);
            Assert.AreEqual(100, list.Count);
            Assert.IsTrue(list.Contains(many.First));
            Assert.IsTrue(list.Contains(many.Last));

            Assert.IsTrue(many.Any(i => { return i.ToInt() == 45; }));
        }
        [Test]
        public void OrderedIntHashCode()
        {
            foreach (var i in one_zero) {
                Assert.IsTrue(i.ToInt().GetHashCode() == i.GetHashCode());
            }
        }
        [Test]
        public void OrderedIntOrder()
        {
            Assert.IsTrue(one_zero.First > one_zero.Last);
            Assert.IsTrue(one_zero.First >= one_zero.Last);
            Assert.IsTrue(ones.First <= ones.Last);
        }

        protected Range<OrderedInteger> MakeRange(int a, int b)
        {
            return new Range<OrderedInteger>(
                new OrderedInteger(a),
                new OrderedInteger(b));
        }
    }
}
