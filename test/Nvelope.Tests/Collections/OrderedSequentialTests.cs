using System;
using NUnit.Framework;
using Nvelope.Collections;

namespace Nvelope.Tests.Collections
{
    [TestFixture]
    class OrderedSequentialTests
    {
        protected OrderedInteger one = new OrderedInteger(1);
        protected OrderedInteger two = new OrderedInteger(2);

        [Test]
        public void Compare()
        {
            // this may be unintuitive, but I don't see any way
            // to get around it.
            Assert.IsFalse(one.Equals(1));

            Assert.AreNotEqual(0, one.CompareTo(null));
            Assert.Throws<NotImplementedException>(
                () => { one.CompareTo(1); });
            Assert.AreNotEqual(0, OrderedInteger.Compare(null, one));
            Assert.AreNotEqual(0, OrderedInteger.Compare(one, null));

            Assert.IsTrue(one < two);
            Assert.IsTrue(one != two);

        }
    }

    /// <summary>
    /// Basically an int, but it implements the features of
    /// IOrderedSequencial.
    /// </summary>
    public class OrderedInteger : OrderedSequential<OrderedInteger>
    {
        protected int Value;

        public OrderedInteger(int i)
        {
            this.Value = i;
        }
        public override int Difference(OrderedInteger other)
        {
            return this.Value - other.Value;
        }
        protected override OrderedInteger Add(int n)
        {
            return new OrderedInteger(this.Value + n);
        }
        public override string ToString()
        {
            return this.Value.ToString();
        }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        public int ToInt() {
            return this.Value;
        }
    }
}
