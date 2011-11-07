using System;
using NUnit.Framework;
using Nvelope.Tabular;

namespace Nvelope.Tests.Tabular
{
    [TestFixture]
    public class CellLocTests
    {
        [Test]
        public void Range()
        {
            var x = new CellLoc(1, 1);
            Assert.AreEqual("([1,1],[1,2],[1,3],[1,4])", x.ColRange(4).Print());
            Assert.AreEqual("([1,1],[2,1],[3,1])", x.RowRange(3).Print());
        }

        [Test]
        public void NegativeRange()
        {
            var x = new CellLoc(5, 5);
            Assert.AreEqual("([5,5],[5,4],[5,3],[5,2],[5,1])", x.ColRange(-5).Print());
            Assert.AreEqual("([5,5],[4,5],[3,5],[2,5],[1,5])", x.RowRange(-5).Print());
        }

        [Test]
        public void Operators()
        {
            var c11 = new CellLoc(1, 1);
            var c12 = new CellLoc(1, 2);
            var c32 = new CellLoc(3, 2);
            Assert.IsFalse(c11 == c12);
            Assert.IsTrue(c11 != c12);
            Assert.IsTrue(c11 == c12.Left());
            Assert.IsTrue(c11.Right() == c12);
            Assert.IsTrue(c12.Below(2) == c32);
            Assert.IsFalse(c12 == c32.Above());

            object o11 = c11;
            Assert.IsTrue(c11.CompareTo(o11) == 0);
            Assert.IsTrue(c11.CompareTo(DateTime.Now) != 0);

        }

        [Test]
        public void To()
        {
            var s = new CellLoc(1, 1);
            var e = new CellLoc(3, 3);
            var locs = s.To(e).Sort();
            var rlocs = e.To(s).Sort();

            Assert.AreEqual(locs.Print(), rlocs.Print());
            Assert.AreEqual("([1,1],[1,2],[1,3],[2,1],[2,2],[2,3],[3,1],[3,2],[3,3])", locs.Print());
        }

        [Test]
        public void ToSelf()
        {
            var s = new CellLoc(5, 5);
            var locs = s.To(s);
            Assert.AreEqual("([5,5])", locs.Print());
        }

        [Test]
        public void AtCol()
        {
            Assert.AreEqual("[2,4]", new CellLoc(2, 18).AtCol(4).ToString());
        }

        [Test]
        public void AtRow()
        {
            Assert.AreEqual("[2,4]", new CellLoc(18, 4).AtRow(2).ToString());                
        }

        [Test]
        public void CellCoordinateConstructor()
        {
            Assert.AreEqual("[0,0]", new CellLoc("A1").Print());
            Assert.AreEqual("[0,1]", new CellLoc("B1").Print());
            Assert.AreEqual("[0,27]", new CellLoc("AB1").Print());
            Assert.AreEqual("[837,32]", new CellLoc("AG838").Print());
        }
    }
}
