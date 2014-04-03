using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class FunctionExtensionTests
    {
        [Test]
        public void Memoize()
        {
            var start = "a";
            Func<int, string> f = i => start + i.ToString();
            // This is the simplest version of Memoize
            var mf = f.Memoize();
            Assert.AreEqual("a0", mf(0));

            start = "b";

            Assert.AreEqual("a0", mf(0));
            Assert.AreEqual("b1", mf(1));
        }
        [Test]
        public void MemoizeFn()
        {
            var start = "a";
            Func<int, string> f = i => start + i.ToString();
            // This version will only cache when called with 3
            var mf = f.Memoize(i => i == 3);

            Assert.AreEqual("a1", mf(1));
            Assert.AreEqual("a3", mf(3));

            // Now we'll change start, which will generate different
            // results for our function. However, since we've memoized it
            // for 3, calling the function with 3 should still return our
            // original values
            start = "b";

            Assert.AreEqual("b1", mf(1));
            Assert.AreEqual("a3", mf(3));
        }

        [Test]
        public void CurryFunc()
        {
            var tolerance = .00001;

            // vi = mussle velocity (m/s)
            // g = gravitaitonal acceleration (m/s**2)
            // angle = angle away from surface (rads?)
            // height = height above target
            // equation from http://en.wikipedia.org/wiki/Range_of_a_projectile
            Func<double, double, double, double, double> CannonRange =
                (g, height, angle, vi) => {
                    var viv = Math.Sin(angle) * vi;
                    var vih = Math.Cos(angle) * vi;
                    return (vih/g) * (viv +
                        Math.Sqrt(Math.Pow(viv, 2) + 2 * g * height));
                };

            Assert.AreEqual(Math.Sqrt(2), CannonRange(1, 1, 0, 1), tolerance);
            Assert.AreEqual(0, CannonRange(1, 1, Math.PI / 2, 1), tolerance);


            var CannonRangeEarth = CannonRange.Curry(9.8);
            Assert.AreEqual(3529.014602789, CannonRangeEarth(-10, Math.PI / 3, 200), tolerance);

            var testspeed = 200;

            var CannonRangeFlat = CannonRangeEarth.Curry(0);
            Assert.AreEqual(0, CannonRangeFlat(0, testspeed), tolerance);
            Assert.AreEqual(810.8952277, CannonRangeFlat(.1, testspeed), tolerance);

            var MaxCannonRangeFlat = CannonRangeFlat.Curry(Math.PI / 4);

            Assert.Greater(MaxCannonRangeFlat(testspeed),
                CannonRangeFlat(Math.PI / 4 + .01, testspeed));
            Assert.Greater(MaxCannonRangeFlat(testspeed),
                CannonRangeFlat(Math.PI / 4 - .01, testspeed));



            // max range a cannon that shoots at supersonic speeds
            // 500 m/s
            var SuperSonicCannonRange = MaxCannonRangeFlat.Curry(500);
            Assert.AreEqual(25510.2040816, SuperSonicCannonRange(), tolerance);
        }
        
        [Test]
        public void CurryAction()
        {
            var a = 0;
            var b = 0;
            var c = 0;
            var d = 0;

            Action<int, int, int, int> Inc =
                (a_inc, b_inc, c_inc, d_inc) => {
                    a += a_inc;
                    b += b_inc;
                    c += c_inc;
                    d += d_inc;
                };
            var Inc2 = Inc.Curry(1).Curry(2).Curry(3);
            Inc2(-1);
            Assert.AreEqual(1, a);
            Assert.AreEqual(2, b);
            Assert.AreEqual(3, c);
            Assert.AreEqual(-1, d);
        }

        [Test]
        public void Dispatch()
        {
            Func<int, string> trueFn = i => "A";
            Func<int, string> falseFn = i => "B";
            Func<int, bool> dispatchFn = i => i == 1;

            var f = dispatchFn.Dispatch(trueFn, falseFn);
            Assert.AreEqual("A", f(1));
            Assert.AreEqual("B", f(2));
        }

        [Test]
        public void Reverse2Args()
        {
            Func<int, string, bool> f = (i, s) => i.ToString() == s;
            var rf = f.ReverseArgs();
            Assert.True(f(1, "1"));
            Assert.True(rf("1", 1));
        }

        [Test]
        public void Reverse3Args()
        {
            Func<int, char, bool, string> f = (i, c, b) => i.ToString() + c + b.ToString();
            var rf = f.ReverseArgs();

            Assert.AreEqual("5sTrue", rf(true, 's', 5));
        }

        [Test]
        public void FilterArg()
        {
            Func<string, string> f = i => i;
            var ff = f.FilterArg(a => a.ToUpper());

            Assert.AreEqual("z", f("z"));
            Assert.AreEqual("Z", ff("z"));

        }

        [Test]
        public void CreateFunc()
        {
            var list = new { A = 1, B = "hello" }.List();

            // This function lets us create a new function from a list of an anonymous type
            // This is pretty much the only way I can think of to create a variable that holds
            // a function that uses an anonymous type as one of its params
            var func = list.CreateFunc(l => l.B);

            // Our func selects just B from our anonymous object, so we should be getting
            // just a list of the B values from our original list here
            Assert.AreEqual("(hello)", list.Select(func).Print());
        }

        [Test]
        public void Cache()
        {
            // The function just returns the current time
            Func<int, DateTime> f = i => DateTime.Now;

            // Cache for 30ms
            var func = f.Memoize(new TimeSpan(0, 0, 0, 0, 30));
            var initialVal = func(1);
            // Results should be equal, since we're getting it from cache
            Assert.AreEqual(initialVal, func(1));
            // Wait until the cache expires
            Thread.Sleep(50);
            // Now the values should be different, since the cache has expired
            Assert.AreNotEqual(initialVal, func(1));
        }

        [Test]
        public void Or()
        {
            Func<int, bool> p = i => i < 3;
            Func<int, bool> q = i => i > 5;
            Func<int, bool> pOrq = p.Or(q);

            Assert.True(pOrq(2));
            Assert.False(pOrq(4));
            Assert.True(pOrq(6));
        }

        [Test]
        public void And()
        {
            Func<int, bool> p = i => i > 3;
            Func<int, bool> q = i => i < 5;
            Func<int, bool> pAndq = p.And(q);

            Assert.False(pAndq(2));
            Assert.True(pAndq(4));
            Assert.False(pAndq(6));
        }

        [Test]
        public void Until()
        {
            var data = new int[] { 1,2,3,4,4 };
            Func<int, int> fn = i => data[i];

            var untilFn = fn.Until(i => i == 4);
            Assert.AreEqual(4, untilFn(1));

            var untilStopsFn = fn.UntilStops();
            // untilFn should iterate until it returns the same value again
            // in this case: 4
            Assert.AreEqual(4, untilStopsFn(0));
        }

        [Test]
        public void Then()
        {
            Func<int, int> times = i => i * i;
            Func<int, int> add = i => i + i;

            var ta = times.Then(add);
            var at = add.Then(times);

            Assert.AreEqual(18, ta(3));
            Assert.AreEqual(36, at(3));
        }

        [Test]
        public void Benchmark()
        {
            Func<int, bool> even = i => i % 2 == 0;

            var time = even.Benchmark(1.And(2), 100);
            Assert.AreNotEqual(int.MinValue, time);
        }

        [Test]
        public void BenchmarkAction()
        {
            var sum = 0;
            Action<int, int> fn = (i, j) => sum += i + j;
            var someNums = 1.Inc().Take(2);
            var data = someNums.ZipToDict(someNums);

            var time = fn.Benchmark(data.Repeat(50));
            Assert.AreNotEqual(int.MinValue, time);
        }

    }
}
