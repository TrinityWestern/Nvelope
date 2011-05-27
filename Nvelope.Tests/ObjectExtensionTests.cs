using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        [Test]
        public void Eq()
        {
            object a = 1;
            object b = 1;
            Assert.False(a == b);
            Assert.True(a.Eq(b));

            b = null;
            Assert.False(a.Eq(b));
            Assert.False(b.Eq(a));
            a = null;
            Assert.True(a.Eq(b));
            Assert.True(b.Eq(a));
        }
    }
}
