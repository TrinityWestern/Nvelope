using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope;

namespace Nvelope.Tests
{
    [TestFixture]
    public class FnTests
    {
        [Test]
        public void Thread()
        {
            var i = 2;
            var res = Fn.Thread(i, j => j + 1, j => j * 2);
            // Should be equal to (2 + 1) * 2
            Assert.AreEqual(6, res);
        }
    }
}
