using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class IComparableExtensionTests
    {
        [Test]
        public void Between()
        {
            var today = new DateTime(2011, 2, 14);
            var yesterday = new DateTime(2011, 2, 13);
            var tommorrow = new DateTime(2011, 2, 15);

            Assert.True(today.Between(yesterday, tommorrow));
            Assert.False(yesterday.Between(today, tommorrow));
            Assert.False(tommorrow.Between(yesterday, today));

            // Should work if we reverse args by accident as well
            Assert.True(today.Between(tommorrow, yesterday));

            // Boundary case - the range should be inclusive
            Assert.True(today.Between(today, tommorrow));
            Assert.True(today.Between(yesterday, today));
            Assert.True(today.Between(today, today));
        }
    }
}
