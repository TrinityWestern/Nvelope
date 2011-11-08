using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace Nvelope.Reactive.Tests
{
    [TestFixture]
    public class FunctionExtensionTests
    {
        [Test]
        public void Memoize()
        {
            // TODO: Figure out a way to test this without using thread timing, or
            // at least reduce the amount of thread timing. It's not necessarily
            // reliable, and at least some of this stuff (ie, the cache invalidator)
            // doesn't necessarily need to be time-based

            var calls = new List<int>();
            Func<int, string> fn = i => {calls.Add(i); return i.ToString(); };
            
            // Create a sequence of 1s generated every X ms
            // This will be the feed we pass into the cache as the cacheinvalidator
            var invalids = Observable.Generate<int, int>(
                initialState: 1,
                condition: t => true,
                iterate: t => 1,
                resultSelector: t => 1,
                timeSelector: t => TimeSpan.FromMilliseconds(30));

            var memfn = fn.Memoize(TimeSpan.FromMilliseconds(100), invalids);
            // When we call once, we should have made one call to the underlying fn
            memfn(1);
            Assert.AreEqual(1, calls.Count());
            // If we immediately call again, we should use the cached value
            memfn(1);
            Assert.AreEqual(1, calls.Count());

            // Now, let's wait until the cacheinvalidator fires
            // This should trigger us to make another call to the underlying fn
            Thread.Sleep(40);
            memfn(1);
            Assert.AreEqual(2, calls.Count());
            // But again, immediately calling it should result in the cached value - no call to the underlying fn
            memfn(1);
            Assert.AreEqual(2, calls.Count());

            // Now, wait until the timeout on the cache expires - this should also generate a call
            // to the underlying fn
            Thread.Sleep(150);
            memfn(1);
            Assert.AreEqual(3, calls.Count());
        }
    }
}
