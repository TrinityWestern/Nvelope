namespace Nvelope.Tests
{
    using System;
    using NUnit.Framework;

    public class EnglishTests
    {
        [Test]
        public void DescribeByMonth()
        {
            var now = new DateTime(2011, 1, 24);
            var tenDays = new DateTime(2011, 2, 3);
            var month = new DateTime(2011, 2, 23);
            var sixMonth = new DateTime(2011, 7, 23);
            var year = new DateTime(2012, 1, 23);
            var yearAndDay = new DateTime(2012, 1, 24);
            var twoYear = new DateTime(2013, 1, 23);
            var thirtyfiveMonth = new DateTime(2013, 12, 23);
            var eightPfiveYear = new DateTime(2019, 7, 23);

            var fn = new Func<DateTime, string>(d => new Interval<DateTime>(now, d).DescribeByMonth());

            Assert.AreEqual("~0 months", fn(tenDays));
            Assert.AreEqual("1 month", fn(month));
            Assert.AreEqual("6 months", fn(sixMonth));
            Assert.AreEqual("1 year", fn(year));
            Assert.AreEqual("~1 year", fn(yearAndDay),
                "If it's close to a year, just call it a year");
            Assert.AreEqual("2 years", fn(twoYear));
            Assert.AreEqual("2 years and 11 months", fn(thirtyfiveMonth));
            Assert.AreEqual("8 years and 6 months", fn(eightPfiveYear));
        }
    }
}
