namespace Nvelope.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Nvelope;

    [TestFixture]
    public class ListExtensionTests
    {
        [Test]
        public void Shift()
        {
            var list = "abcdef";
            var victims = new char[] { 'c', 'e' };
            var res = list.Shift(victims, -1).ToSeperatedList(string.Empty);
            Assert.AreEqual("acbedf", res);

            victims = new char[] { 'a', 'b' };
            res = list.Shift(victims, -5).ToSeperatedList(string.Empty);
            Assert.AreEqual("abcdef", res);

            victims = new char[] { 'b', 'd' };
            res = list.Shift(victims, 1).ToSeperatedList(string.Empty);
            Assert.AreEqual("acbedf", res);

            victims = new char[] { 'e', 'f' };
            res = list.Shift(victims, 10).ToSeperatedList(string.Empty);
            Assert.AreEqual("abcdef", res);

            res = list.Shift('f'.List(), 100).ToSeperatedList(string.Empty);
            Assert.AreEqual("abcdef", res);
        }

        [Test]
        public void IndexOfEach()
        {
            var list = "abcdef";
            var indices = list.IndexOfEach('a', 'b', 'c');
            Assert.AreEqual("(0,1,2)", indices.Print());

            indices = list.IndexOfEach('d', 'f', 'z');
            Assert.AreEqual("(3,5,-1)", indices.Print());

            // Should only return the first index of each thing
            list = "aabcd";
            Assert.AreEqual("(0)", list.IndexOfEach('a').Print());
        }

        [Test]
        public void Repeat()
        {
            Assert.AreEqual("(a,b,c,a,b,c)", "abc".Repeat().Take(6).Print());
        }

        [Test]
        public void Except()
        {
            Assert.AreEqual("(a,b,d)", "abcd".Except('c').Print());
            Assert.AreEqual("(a,b,c)", "abc".Except('z').Print());
        }

        [Test]
        public void Rest()
        {
            Assert.AreEqual("(b,c)", "abc".Rest().Print());
            Assert.AreEqual("()", "a".Rest().Print());
            Assert.AreEqual("()", string.Empty.Rest().Print());
        }

        [Test]
        public void ToSeperatedList()
        {
            Assert.AreEqual("1,2,3,4", 1.To(4).ToSeperatedList(","));
            Assert.AreEqual("1, 2, 3, 4", 1.To(4).ToSeperatedList(", "));
            Assert.AreEqual("1234", 1.To(4).ToSeperatedList(string.Empty));
            Assert.AreEqual(string.Empty, new int[] { }.ToSeperatedList(string.Empty));
            Assert.AreEqual(string.Empty, new int[] { }.ToSeperatedList(","));
        }

        [Test]
        public void AreAllEqual()
        {
            Assert.IsTrue(string.Empty.AreAllEqual());
            Assert.IsTrue("1111".AreAllEqual());
            Assert.IsFalse("1112".AreAllEqual());
        }

        [Test]
        public void Numbers()
        {
            var list = new int[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(1, list.First());
            Assert.AreEqual(2, list.Second());
            Assert.AreEqual(3, list.Third());
            Assert.AreEqual(4, list.Fourth());
            Assert.AreEqual(5, list.Fifth());
        }

        [Test]
        public void NumbersOr()
        {
            var list = new int[] { 1, 2, 3, 4, 5 };
            Assert.AreEqual(1, list.FirstOr(-1));
            Assert.AreEqual(2, list.SecondOr(-1));
            Assert.AreEqual(3, list.ThirdOr(-1));
            Assert.AreEqual(4, list.FourthOr(-1));
            Assert.AreEqual(5, list.FifthOr(-1));

            Assert.AreEqual(2, list.ElementOr(1, -1));
            Assert.AreEqual(-1, list.ElementOr(10, -1));

            list = new int[] { };
            Assert.AreEqual(-1, list.FirstOr(-1));
            Assert.AreEqual(-1, list.SecondOr(-1));
            Assert.AreEqual(-1, list.ThirdOr(-1));
            Assert.AreEqual(-1, list.FourthOr(-1));
            Assert.AreEqual(-1, list.FifthOr(-1));
        }

        [Test]
        public void AtLeast()
        {
            var list = new int[] { 1, 2, 3 };
            Assert.True(list.AtLeast(2));
            Assert.True(list.AtLeast(3));
            Assert.False(list.AtLeast(4));

            Assert.True(list.AtLeast(2, i => i < 3));
            Assert.False(list.AtLeast(3, i => i < 3));

            // Should work for infinite sequences as well
            Assert.True(1.Inc().AtLeast(2));
            var interval = new Interval<int>(3, 7);
            Assert.True(1.Inc().AtLeast(2, i => interval.Contains(i)));
        }

        [Test]
        public void For()
        {
            var idx = new Dictionary<char, int?>();
            idx.Add('a', null);
            idx.Add('b', null);
            idx.Add('c', null);

            "abc".Chars().For((i, c) => idx[c] = i);
            Assert.AreEqual("([a,0],[b,1],[c,2])", idx.Print());
        }

        [Test]
        public void Slice()
        {
            var list = new int[] { 0, 1, 2, 3, 4, 5 };
            Assert.AreEqual("(3,4,5)", list.Slice(3, 5).Print());
            Assert.AreEqual("(0)", list.Slice(0, 0).Print());
            Assert.AreEqual("(5)", list.Slice(5, 5).Print());
            Assert.AreEqual("(4,3,2)", list.Slice(4, 2).Print());
        }

        [Test]
        public void Chop()
        {
            var list = new int[] { 0, 1, 2 };
            Assert.AreEqual("(0,1)", list.Chop().Print());
            Assert.AreEqual("()", 1.List().Chop().Print());
            Assert.AreEqual("()", new List<int>().Chop().Print());
        }

        [Test]
        public void FirstOr()
        {
            var list = new string[] { "a", "b" };
            Assert.AreEqual("a", list.FirstOr("z"));
            list = new string[] { };
            Assert.AreEqual("z", list.FirstOr("z"));

            list = new string[] { "aa", "b", "a", "bb" };
            Assert.AreEqual("b", list.FirstOr(s => s.StartsWith("b"), "z"));
            Assert.AreEqual("z", list.FirstOr(s => s.StartsWith("c"), "z"));
        }

        [Test]
        public void IsSameAs()
        {
            var list = new int[] { 1, 2, 3 };
            var other = new int[] { 1, 2, 3 };
            Assert.IsTrue(list.IsSameAs(other));
            Assert.IsFalse(list.IsSameAs(other.Rest()));
            Assert.IsFalse(list.IsSameAs(other.Skip(1)));
        }

        [Test]
        public void Zip()
        {
            var ints = new int[] { 1, 3, 5 }.ToList();
            var strs = new string[] { "one", "three", "five" }.ToList();

            var res = ints.Zip(strs, (i, s) => i.ToString() + ":" + s);

            Assert.AreEqual("(1:one,3:three,5:five)", res.Print());
        }

        [Test]
        public void Zip_UnequalLengths()
        {
            var ints = new int[] { 1, 2, 3 };
            var strs = new string[] { "a", "b" };

            Assert.AreEqual("(1a,2b)", ints.Zip(strs, (i, s) => i.ToString() + s).Print());
            Assert.AreEqual("(a1,b2)", strs.Zip(ints, (s, i) => s + i.ToString()).Print());
        }

        [Test]
        public void Take()
        {
            var ints = new int[] { 1, 2, 3 };

            // Test both longer and shorter than source list
            Assert.AreEqual("(1,2)", ints.Take(2, 0).Print());
            Assert.AreEqual("(1,2,3,0,0,0)", ints.Take(6, 0).Print());
        }

        [Test]
        public void And()
        {
            Assert.AreEqual("(a,b,c,d)", "ab".Take(2).And("cd").Print());
            Assert.AreEqual("(a,b,c)", "ab".Take(2).And('c').Print());

            Assert.AreEqual("(1,3)", 1.And(3).Print());
            Assert.AreEqual("(1,2,3)", 1.And(2).And(3).Print());
        }

        [Test]
        public void Or()
        {
            var empty = new int[] { };
            var a = new int[] { 1, 2, 3 };
            var b = new int[] { 4, 5, 6 };
            Assert.AreEqual("(1,2,3)", a.Or(b).Print());
            Assert.AreEqual("(4,5,6)", empty.Or(b).Print());
        }

        [Test]
        public void Interleave()
        {
            var odd = new int[] { 1, 3, 5, 7 };
            var shortEven = new int[] { 2, 4 };
            var longEven = new int[] { 2, 4, 6, 8, 10 };
            var zeros = new int[] { 0 }.Repeat();

            Assert.AreEqual("(1,2,3,4)", odd.Interleave(shortEven).Print());
            Assert.AreEqual("(1,2,3,4,5,6,7,8)", odd.Interleave(longEven).Print());
            Assert.AreEqual("(1,0,3,0,5,0,7,0)", odd.Interleave(zeros).Print());
        }

        [Test]
        public void Interpose()
        {
            var i = new int[] { 1, 3, 5 };
            Assert.AreEqual("(1,0,3,0,5)", i.Interpose(0).Print());
        }

        [Test]
        public void Partition()
        {
            var i = new int[] { 1, 2, 3, 4, 5 };
            var parts = i.Partition(2);
            Assert.AreEqual("((1,2),(3,4),(5))", parts.Print());
        }

        [Test]
        public void PartitionWithMultipleSizes()
        {
            var i = 1.Inc().Take(10);
            var parts = i.Partition(1.Inc().Take(3));
            Assert.AreEqual("((1),(2,3),(4,5,6),(7),(8,9),(10))", parts.Print());
            
            // Make sure we can select 0-length lists
            Assert.AreEqual("((),(1,2,3,4,5),(),(6,7,8,9,10))", i.Partition(0.And(5)).Print());
        }

        [Test]
        public void Scan()
        {
            var data = 1.Inc().Take(3);
            var res = data.Scan("a", (s, i) => s + i.ToString());
            Assert.AreEqual("(a1,a12,a123)", res.Print());
        }

        [Test]
        public void TakeNth()
        {
            var i = new int[] { 1, 2, 3, 4, 5, 6 };
            Assert.AreEqual("(2,4,6)", i.TakeNth(2).Print());
            Assert.AreEqual("(3,6)", i.And(7).TakeNth(3).Print());
        }

        [Test]
        public void Intervals()
        {
            var i = new int[] { 1, 2, 4, 7, 11 };
            
            // Compute the distance between each successive element, and return that 
            // as a list: the diff between 2 and 1 is 1; between 4 and 2 is 2, and so on
            Assert.AreEqual("(1,2,3,4)", i.Intervals((x, y) => y - x).Print());
        }

        [Test]
        public void HavingMax()
        {
            var strs = new string[] { "dddd", "ccc", "bb", "a" };
            Assert.AreEqual("dddd", strs.HavingMax(s => s.Length));
            
            // If there's 2 things that have the max value, one of them should be 
            // returned (which one is undefined)
            Assert.AreEqual(4, strs.And("eeee").HavingMax(s => s.Length).Length);
        }

        [Test]
        public void Iterate()
        {
            Assert.AreEqual("(1,2,3,4)", 1.Iterate(i => ++i).Take(4).Print());
        }

        [Test]
        public void TakeByGroup()
        {
            var i = new string[] { "a", "b", "c", "d", "aa", "bb", "cc", "dd" };
            Assert.AreEqual("(a,b,c,d)", i.TakeByGroup(s => s.Length, 1).Print());
            Assert.AreEqual("(a,b,c,d)", i.TakeByGroup(s => s.Length, 2).Print());
            Assert.AreEqual("(a,b,c,d)", i.TakeByGroup(s => s.Length, 3).Print());
            Assert.AreEqual("(a,b,c,d)", i.TakeByGroup(s => s.Length, 4).Print());
            Assert.AreEqual("(a,b,c,d,aa,bb,cc,dd)", i.TakeByGroup(s => s.Length, 5).Print());
        }

        [Test]
        public void Copy()
        {
            var data = new int[][]
            {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5, 6 },
                new int[] { 7, 8, 9 }
            };

            var original = new List<List<int>>();
            foreach (var row in data)
            {
                original.Add(row.ToList());
            }

            var copy = original.Copy();
            
            // the copy contains the same items as the original
            // so if you modify the items they are modified in both
            Assert.AreEqual(original[1][1], copy[1][1]);
            original[1][1] = 0;
            Assert.AreEqual(original[1][1], copy[1][1]);
            
            // if you remove an item in the original list the item
            // is not removed in the copy
            original.Remove(original[2]);
            Assert.AreEqual(2, original.Count());
            Assert.AreEqual(3, copy.Count());

            // if you switch out an item in one copy it does not
            // modify the other copy
            Assert.AreEqual(original[0][0], copy[0][0]);
            original[0] = data[2].ToList();
            Assert.IsFalse(original[0][0] == copy[0][0]);
        }

        [Test]
        public void EndManipulation()
        {
            var list = new List<int>();
            list.Push(1);
            list.Push(0);
            list.Push(-1);
            Assert.AreEqual("(1,0,-1)", list.Print());
            Assert.AreEqual(-1, list.Pop());
            Assert.AreEqual(1, list.Unshift());
            list.Shift(5);
            Assert.AreEqual("(5,0)", list.Print());
        }

        [Test]
        public void Print()
        {
            var list = new Dictionary<string, object>[]
            {
                new Dictionary<string, object> { { "name", "George" }, { "size", 34 } },
                new Dictionary<string, object> { { "name", "Alice" }, { "size", 29 } },
                new Dictionary<string, object> { { "name", "Dan" }, { "size", 30 } },
            };
            Assert.AreEqual(
                "(([name,George],[size,34]),([name,Alice],[size,29]),([name,Dan],[size,30]))",
                list.Print());
        }

        [Test]
        public void Paginate()
        {
            var list = 1.To(20);
            var page = list.Paginate(3, 3);
            Assert.AreEqual(7.To(9), page);
        }

        [Test]
        public void LastOr()
        {
            Assert.AreEqual('c', "abc".LastOr('x'));
            Assert.AreEqual('x', string.Empty.LastOr('x'));
        }

        [Test]
        public void Flatten()
        {
            var data = new int[][]
            {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5, 6 },
                new int[] { 7, 8, 9 }
            };

            Assert.AreEqual(1.To(9), data.Flatten());
        }

        [Test]
        public void In()
        {
            var list = 1.To(3);
            Assert.IsFalse(0.In(1, 2, 3));
            Assert.IsTrue(1.In(1, 2, 3));
            Assert.IsTrue(3.In(1, 2, 3));
            Assert.IsFalse(4.In(1, 2, 3));
        }
    }
}
