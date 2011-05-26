using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope;

namespace Nvelope.Tests
{
    [TestFixture]
    class IntervalTests
    {
        DateTime A = new DateTime(2011, 1, 1);
        DateTime B = new DateTime(2011, 2, 2);
        DateTime C = new DateTime(2011, 3, 3);
        DateTime D = new DateTime(2011, 4, 4);
        DateTime E = new DateTime(2011, 5, 5);

        [Test]
        public void ContainsItem()
        {
            var ac = new Interval<DateTime>(A, C);
            Assert.True(ac.Contains(A));
            Assert.True(ac.Contains(B));
            Assert.True(ac.Contains(C));
            Assert.True(ac.Flip().Contains(C));
            Assert.False(ac.Contains(D));
            Assert.False(ac.Contains(A.AddDays(-1)));
        }

        [Test]
        public void ContainsInterval()
        {
            var ad = new Interval<DateTime>(A, D);
            var bc = new Interval<DateTime>(B, C);
            Assert.IsTrue(ad.Contains(bc));
            Assert.IsTrue(ad.Contains(ad));
            Assert.IsFalse(bc.Contains(ad));

            var ac = new Interval<DateTime>(A, C);
            var bd = new Interval<DateTime>(B, D);
            Assert.IsFalse(ac.Contains(bd));
            Assert.IsFalse(bd.Contains(ac));

            var da = ad.Flip();
            Assert.IsTrue(da.Contains(bc));
        }

        [Test]
        public void Equals()
        {
            var ad = new Interval<DateTime>(A, D);
            var da = new Interval<DateTime>(D, A);
            Assert.IsTrue(ad.Equals(da.Flip()));
        }


        [Test]
        public void Overlap()
        {
            var ab = new Interval<DateTime>(A, B);
            var ac = new Interval<DateTime>(A, C);
            var bd = new Interval<DateTime>(B, D);
            var cd = new Interval<DateTime>(C, D);
            var over = ac.Overlap(bd);
            Assert.AreEqual(over.Start, B);
            Assert.AreEqual(over.End, C);
            over = bd.Overlap(ac);
            Assert.AreEqual(over.Start, B);
            Assert.AreEqual(over.End, C);

            over = ab.Overlap(cd);
            Assert.IsNull(over);
            over = ac.Overlap(cd);
            Assert.IsNull(over);

            over = new Interval<DateTime>(A, D).Overlap(new Interval<DateTime>(B, C));
            Assert.AreEqual(over.Start, B);
            Assert.AreEqual(over.End, C);
        }

        [Test]
        public void Gap()
        {
            var ab = new Interval<DateTime>(A, B);
            var cd = new Interval<DateTime>(C, D);
            var bc = new Interval<DateTime>(B, C);

            var gap = ab.Gap(cd);
            Assert.AreEqual(bc.Print(), gap.Print());

            gap = ab.Gap(bc);
            Assert.IsNull(gap);
        }

        [Test]
        public void Overlaps()
        {
            var ac = new Interval<DateTime>(A, C);
            var bd = new Interval<DateTime>(B, D);
            var ce = new Interval<DateTime>(C, E);

            var res = ac.And(bd).And(ce).Overlaps();
            Assert.AreEqual("(2/2/2011 12:00:00 AM—3/3/2011 12:00:00 AM,3/3/2011 12:00:00 AM—4/4/2011 12:00:00 AM)", res.Print());
            Assert.AreEqual("()", ac.And(ce).Overlaps().Print());
        }

        [Test]
        public void Gaps()
        {
            var ab = new Interval<DateTime>(A, B);
            var cd = new Interval<DateTime>(C, D);
            var de = new Interval<DateTime>(D, E);

            var res = ab.And(cd).And(de).Gaps();
            Assert.AreEqual("(2/2/2011 12:00:00 AM—3/3/2011 12:00:00 AM)", res.Print());
            Assert.AreEqual("()", cd.And(de).Gaps().Print());
        }
    }
}
