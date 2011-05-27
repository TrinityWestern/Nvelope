using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class TimeSpanExtensionTests
    {
        [Test]
        public void RoundToYearsTest()
        {
            TimeSpan oneYear = new TimeSpan(365, 0, 0, 0);
            Assert.AreEqual(1, oneYear.RoundToYears());

            TimeSpan sixMonths = new TimeSpan(184, 0, 0, 0);
            Assert.AreEqual(1, sixMonths.RoundToYears());

            TimeSpan oneMonth = new TimeSpan(31, 0, 0, 0);
            Assert.AreEqual(0, oneMonth.RoundToYears());
        }
    }
}
