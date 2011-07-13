using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class NumberExtensionTests
    {
        [Test]
        public void IsOddOrEvenTest()
        {
            int zero = 0;
            int one = 1;
            int two = 2;

            Assert.IsTrue(zero.IsEven());
            Assert.IsFalse(zero.IsOdd());

            Assert.IsTrue(one.IsOdd());
            Assert.IsFalse(one.IsEven());

            Assert.IsFalse(two.IsOdd());
            Assert.IsTrue(two.IsEven());
        }
        [Test]
        public void Mod()
        {
            Assert.AreEqual(2, (-3).Mod(5));
            Assert.AreEqual(2, 2.Mod(5));
            Assert.AreEqual(0, 5.Mod(5));
            Assert.AreEqual(0, (-5).Mod(5));
            Assert.AreEqual(-1, 5.Mod(-2));
            Assert.AreEqual(0, 5.Mod(-5));
            Assert.Throws<DivideByZeroException>(() => 5.Mod(0));
        }
        [Test]
        public void DivDown()
        {
            Assert.AreEqual(2, 6.DivDown(3));
            Assert.AreEqual(2, 7.DivDown(3));
            Assert.AreEqual(-2, (-6).DivDown(3));
            Assert.AreEqual(-2, (-5).DivDown(3));
            Assert.AreEqual(0, 0.DivDown(3));
            Assert.Throws<DivideByZeroException>(() => 4.DivDown(0));
        }
        
        [Test]
        public void To()
        {
            Assert.AreEqual("(0,1,2)", 0.To(2).Print());
            Assert.AreEqual("(1,3,5,7)", 1.To(7, 2).Print());
            Assert.AreEqual("(1,3,5,7)", 1.To(8, 2).Print());

            // Make sure it works backwards
            Assert.AreEqual("(5,4,3)", 5.To(3).Print());
            Assert.AreEqual("(5,3,1)", 5.To(1, -2).Print());

        }

        [Test]
        public void RoundTo()
        {   
            Assert.AreEqual(0.05m, 0.0512m.RoundTo(2));
            Assert.AreEqual(0.05m, 0.045m.RoundTo(2));
            Assert.AreEqual(0.04m, 0.045m.RoundTo(2, MidpointRounding.ToEven));            
        }

        [Test]
        public void IsAbout()
        {
            Assert.True(1.01m.IsAbout(1.0m, .1m));
            Assert.False(1.01m.IsAbout(1.0m, .001m));

            Assert.False(2m.IsAbout(1m, .5m));
        }

        [Test]
        public void AsPercentage()
        {
            Assert.AreEqual("1.50%", 0.015m.AsPercentage());
            Assert.AreEqual("100.00%", 1m.AsPercentage());
            Assert.AreEqual("0.13%", 0.00125m.AsPercentage());
        }

        [Test]
        public void IncAddition()
        {
            int i = 1;
            IEnumerable<int> increment = i.Inc(1);
            var x = 0;
            foreach (int j in increment)
            {
                Assert.AreEqual(i+x, j);
                x++;
                if (x > 100)
                    break;
            }
        }

        [Test]
        public void IncSubtraction()
        {
            int i = 100;
            IEnumerable<int> increment = i.Inc(-1);
            var x = 0;
            foreach (int j in increment)
            {
                Assert.AreEqual(i - x, j);
                x++;
                if (x > 100)
                    break;
            }
        }

        [Test]
        public void Times()
        {
            int i = 5;
            int val = 0;
            Action act = () => val++;
            i.Times(act);
            Assert.AreEqual(5, val); 
        }
    }
}
