using System;
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

        [Test]
        public void DescribeLength()
        {
            var now = new DateTime(2011, 1, 24);
            var second = now.AddSeconds(1);
            var fiveSecond = now.AddSeconds(5);
            var minute = now.AddMinutes(1);
            var fiveMinute = now.AddMinutes(5);
            var hour = now.AddHours(1);
            var fiveHour = now.AddHours(5);
            var day = now.AddDays(1);
            var fiveDay = now.AddDays(5);
            var week = now.AddDays(7);
            var twoWeek = now.AddDays(14);
            var month = new DateTime(2011, 2, 24);
            var sixMonth = new DateTime(2011, 7, 24);
            var year = new DateTime(2012, 1, 24);
            var yearAndDay = new DateTime(2012, 1, 25);
            var eighteenMonth = new DateTime(2012, 7, 24);
            var twoYear = new DateTime(2013, 1, 24);
            var thirtyfiveMonth = new DateTime(2013, 12, 24);
            var eightPfiveYear = new DateTime(2019, 7, 24);

            var fn = new Func<DateTime, string>(d => d.Subtract(now).DescribeLength());

            Assert.AreEqual("1 second", fn(second));
            Assert.AreEqual("5 seconds", fn(fiveSecond));
            Assert.AreEqual("1 minute", fn(minute));
            Assert.AreEqual("5 minutes", fn(fiveMinute));
            Assert.AreEqual("1 hour", fn(hour));
            Assert.AreEqual("5 hours", fn(fiveHour));
            Assert.AreEqual("1 day", fn(day));
            Assert.AreEqual("5 days", fn(fiveDay));
            Assert.AreEqual("1 week", fn(week));
            Assert.AreEqual("2 weeks", fn(twoWeek));
            Assert.AreEqual("1 month", fn(month));
            Assert.AreEqual("6 months", fn(sixMonth));
            Assert.AreEqual("1 year", fn(year));
            Assert.AreEqual("1 year", fn(yearAndDay),
                "If it's close to a year, just call it a year");
            Assert.AreEqual("18 months", fn(eighteenMonth),
                "Small periods of years should be measured in months, unless they're complete years");
            Assert.AreEqual("2 years", fn(twoYear));
            Assert.AreEqual("35 months", fn(thirtyfiveMonth));
            Assert.AreEqual("8 years", fn(eightPfiveYear),
                "If it's a big enough number of years, don't bother counting it in months");
        }
    }
}
