using System;
using System.Linq;
using NUnit.Framework;

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
        public void OverlapWith0LengthIntervals()
        {
            var ab = new Interval<DateTime>(A, B);            
            var bd = new Interval<DateTime>(B, D);
            var cd = new Interval<DateTime>(C, D);

            var over = ab.Overlap(bd, true);
            Assert.NotNull(over);
            Assert.AreEqual(B, over.Start);
            Assert.AreEqual(B, over.End);

            over = ab.Overlap(cd, true);
            Assert.Null(over);
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
        public void Merge()
        {
            var ad = new Interval<DateTime>(A, D);
            var ab = new Interval<DateTime>(A, B);
            var ce = new Interval<DateTime>(C, E);

            var res = ab.And(ce).Merge();
            Assert.AreEqual(2, res.Count());
            Assert.AreEqual(A, res.First().Start);
            Assert.AreEqual(B, res.First().End);
            Assert.AreEqual(C, res.Second().Start);
            Assert.AreEqual(E, res.Second().End);

            res = ad.And(ab).And(ce).Merge();
            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(A, res.First().Start);
            Assert.AreEqual(E, res.First().End);
        }

        [Test]
        public void Overlaps()
        {
            var ac = new Interval<DateTime>(A, C);
            var bd = new Interval<DateTime>(B, D);
            var ce = new Interval<DateTime>(C, E);

            var res = ac.And(bd).And(ce).Overlaps();
            Assert.AreEqual(2, res.Count());
            Assert.AreEqual(B, res.First().Start);
            Assert.AreEqual(C, res.First().End);
            Assert.AreEqual(C, res.Second().Start);
            Assert.AreEqual(D, res.Second().End);
            Assert.AreEqual("()", ac.And(ce).Overlaps().Print());
        }

        [Test]
        public void Gaps()
        {
            var ab = new Interval<DateTime>(A, B);
            var cd = new Interval<DateTime>(C, D);
            var de = new Interval<DateTime>(D, E);

            var res = ab.And(cd).And(de).Gaps();
            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(B, res.First().Start);
            Assert.AreEqual(C, res.First().End);            
            Assert.AreEqual("()", cd.And(de).Gaps().Print());
        }
    }
}
