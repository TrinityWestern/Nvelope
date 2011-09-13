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
            var month = new DateTime(2011, 2, 24);
            var sixMonth = new DateTime(2011, 7, 24);
            var year = new DateTime(2012, 1, 24);
            var yearAndDay = new DateTime(2012, 1, 25);
            var twoYear = new DateTime(2013, 1, 24);
            var thirtyfiveMonth = new DateTime(2013, 12, 24);
            var eightPfiveYear = new DateTime(2019, 7, 24);

            var fn = new Func<DateTime, string>(d => d.Subtract(now).Approximate().ToEnglish(true));

            Assert.AreEqual("one second", fn(second));
            Assert.AreEqual("5 seconds", fn(fiveSecond));
            Assert.AreEqual("one minute", fn(minute));
            Assert.AreEqual("5 minutes", fn(fiveMinute));
            Assert.AreEqual("one hour", fn(hour));
            Assert.AreEqual("5 hours", fn(fiveHour));
            Assert.AreEqual("one day", fn(day));
            Assert.AreEqual("5 days", fn(fiveDay));
            Assert.AreEqual("one month", fn(month));
            Assert.AreEqual("6 months", fn(sixMonth));
            Assert.AreEqual("one year", fn(year));
            Assert.AreEqual("one year", fn(yearAndDay),
                "If it's close to a year, just call it a year");
            Assert.AreEqual("two years", fn(twoYear));
            Assert.AreEqual("three years", fn(thirtyfiveMonth));
            Assert.AreEqual("9 years", fn(eightPfiveYear),
                "If it's a big enough number of years, don't bother counting it in months");


            var digit = new Func<DateTime, string>(d => d.Subtract(now).Approximate().ToEnglish(false));

            Assert.AreEqual("1 second", digit(second));
            Assert.AreEqual("5 seconds", digit(fiveSecond));
            Assert.AreEqual("1 minute", digit(minute));
            Assert.AreEqual("5 minutes", digit(fiveMinute));
            Assert.AreEqual("1 hour", digit(hour));
            Assert.AreEqual("5 hours", digit(fiveHour));
            Assert.AreEqual("1 day", digit(day));
            Assert.AreEqual("5 days", digit(fiveDay));
            Assert.AreEqual("1 month", digit(month));
            Assert.AreEqual("6 months", digit(sixMonth));
            Assert.AreEqual("1 year", digit(year));
            Assert.AreEqual("1 year", digit(yearAndDay),
                "If it's close to a year, just call it a year");
            Assert.AreEqual("2 years", digit(twoYear));
            Assert.AreEqual("3 years", digit(thirtyfiveMonth));
            Assert.AreEqual("9 years", digit(eightPfiveYear),
                "If it's a big enough number of years, don't bother counting it in months");
        }
        [Test]
        public void ApproximateNoTime()
        {
            Assert.AreEqual(new TimeSpan(0), new TimeSpan(0, 0, 0, 0, 232).Approximate());
        }

        [Test]
        public void ApproximateFourSeconds()
        {
            var expect = new TimeSpan(0, 0, 4);
            var threeLongSeconds = new TimeSpan(0, 0, 0, 3, 600);

            Assert.AreEqual(expect, threeLongSeconds.Approximate());
            Assert.AreEqual(expect, expect.Approximate());


        }
        [Test]
        public void ApproximateDays()
        {
            var day = new TimeSpan(1, 0, 0, 0);
            var almostFullDay = new TimeSpan(20, 0, 0);
            var almostOneAndAHalf = new TimeSpan(1, 11, 59, 0);

            Assert.AreNotEqual(day, almostFullDay.Approximate());
            Assert.AreEqual(day, almostOneAndAHalf.Approximate());
            Assert.AreEqual(day + day, (almostFullDay + almostOneAndAHalf).Approximate());


        }
        [Test]
        public void ApproximateTwoMonths()
        {
            var expect = TimeSpanExtensions.ApproximateMonth + TimeSpanExtensions.ApproximateMonth;
            var twoishMonths = new TimeSpan(50, 0, 0, 0);
            var almostThree = new TimeSpan(74, 23, 59, 59);

            Assert.AreEqual(expect, twoishMonths.Approximate());
            Assert.AreEqual(expect, almostThree.Approximate());
        }

        [Test]
        public void ApproximateTwoYears()
        {
            var oneYear = TimeSpanExtensions.ApproximateYear;
            var twoYears = oneYear + oneYear;
            var threeYears = oneYear + twoYears;

            var barelyTwo = new TimeSpan(547, 12, 0, 0);
            var almostThree = new TimeSpan(912, 11, 59, 59);
            var oneMinute = new TimeSpan(0, 1, 0);

            Assert.AreEqual(oneYear, (barelyTwo - oneMinute).Approximate());
            Assert.AreEqual(twoYears, barelyTwo.Approximate());
            Assert.AreEqual(twoYears, almostThree.Approximate());
            Assert.AreEqual(threeYears, (almostThree + oneMinute).Approximate());
        }

        [Test]
        public void ApproximateEnglish()
        {
            var barelyTwo = new TimeSpan(547, 12, 0, 0);
            var oneMinute = new TimeSpan(0, 1, 0);

            Assert.AreEqual("now", new TimeSpan(0, 0, 0, 0, 232).Approximate().ToEnglish(true));
            Assert.AreEqual("one year", (barelyTwo - oneMinute).Approximate().ToEnglish(true));
            Assert.AreEqual("two years", barelyTwo.Approximate().ToEnglish(true));

            Assert.AreEqual("now", new TimeSpan(0, 0, 0, 0, 232).Approximate().ToEnglish(false));
            Assert.AreEqual("1 year", (barelyTwo - oneMinute).Approximate().ToEnglish(false));
            Assert.AreEqual("2 years", barelyTwo.Approximate().ToEnglish(false));
        }

        [Test]
        public void Multiply()
        {
            var oneMinute = new TimeSpan(0, 1, 0);
            var fourMinutes = new TimeSpan(0, 4, 0);

            Assert.AreEqual(fourMinutes, oneMinute.Multiply(4));
        }
    }
}
