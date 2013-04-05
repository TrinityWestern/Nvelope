using System;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        [Test]
        public void OnSameDayAsTest()
        {
            DateTime a = DateTime.Now;
            DateTime b = DateTime.Now.AddSeconds(30);

            Assert.IsTrue(a.OnSameDayAs(b));
        }

        [Test]
        public void OnDifferentDayThanTest()
        {
            DateTime a = DateTime.Now;
            DateTime b = DateTime.Now.AddDays(1);

            Assert.IsFalse(a.OnSameDayAs(b));
        }

        [Test]
        public void ToIsoDate()
        {
            DateTime? d = null;
            Assert.AreEqual("", d.ToIsoDate());
            d = new DateTime(2011, 02, 24, 22, 10, 5);
            Assert.AreEqual("2011-02-24", d.ToIsoDate());
        }

        [Test]
        public void ToIsoDateTime()
        {
            DateTime? d = null;
            Assert.AreEqual("", d.ToIsoDateTime());
            d = new DateTime(2011, 02, 24, 22, 10, 5);
            Assert.AreEqual("2011-02-24 22:10:05", d.ToIsoDateTime());
        }

        [Test]
        public void SameMinuteAs()
        {
            var now = DateTime.Now.Date.AddHours(1).AddMinutes(11);

            Assert.True(now.SameMinuteAs(now.AddSeconds(20)));
            Assert.True(now.SameMinuteAs(now.AddSeconds(55)));
            Assert.False(now.SameMinuteAs(now.AddSeconds(70)));
            Assert.False(now.SameMinuteAs(now.AddSeconds(-10)));
        }

        [Test]
        public void IsToday()
        {
            DateTime now = DateTime.Now;
            DateTime midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            Assert.IsTrue(now.IsToday());
            Assert.IsTrue(midnight.IsToday());
        }

        [Test]
        public void MonthsDifference()
        {
            Assert.AreEqual(0, new DateTime(2011, 2, 2).MonthsDifference(new DateTime(2011, 3, 1)));
            Assert.AreEqual(1, new DateTime(2011, 2, 2).MonthsDifference(new DateTime(2011, 3, 2)));
            Assert.AreEqual(1, new DateTime(2011, 2, 2).MonthsDifference(new DateTime(2011, 3, 3)));
            Assert.AreEqual(1, new DateTime(2011, 1, 30).MonthsDifference(new DateTime(2011, 3, 1)));

            Assert.AreEqual(0, new DateTime(2010, 12, 12).MonthsDifference(new DateTime(2011, 1, 1)));
            Assert.AreEqual(1, new DateTime(2010, 12, 12).MonthsDifference(new DateTime(2011, 1, 31)));

            Assert.AreEqual(25, new DateTime(2010, 1, 10).MonthsDifference(new DateTime(2012, 2, 12)));
        }

        [Test]
        public void DaysInYear()
        {
            Assert.AreEqual(365, new DateTime(2011, 1, 1).DaysInYear());
            Assert.AreEqual(366, new DateTime(2012, 1, 1).DaysInYear());
        }

        [Test]
        public void IsEndOfMonth()
        {
            Assert.True(new DateTime(2011, 1, 31).IsEndOfMonth());
            Assert.IsFalse(new DateTime(2011, 2, 1).IsEndOfMonth());
            Assert.IsFalse(new DateTime(2011, 1, 20).IsEndOfMonth());
        }

        [Test]
        public void SameMonthAs()
        {
            var feb1 = new DateTime(2011, 2, 1);
            var feb2 = new DateTime(2011, 2, 2);
            var feb28 = new DateTime(2011, 2, 28);
            var jan31 = new DateTime(2011, 1, 31);
            var mar1 = new DateTime(2011, 3, 1);
            Assert.True(feb1.SameMonthAs(feb2));
            Assert.True(feb1.SameMonthAs(feb28));
            Assert.False(feb1.SameMonthAs(jan31));
            Assert.False(feb1.SameMonthAs(mar1));
        }

        [Test]
        public void DaysInMonth()
        {
            DateTime d = new DateTime(2008, 02, 28);
            int x = d.DaysInMonth();
            DateTime e = new DateTime(2010, 02, 28);
            int y = e.DaysInMonth();

            Assert.AreEqual(29, x);
            Assert.AreEqual(28, y);

        }

        [Test]
        public void ToFriendlyDate()
        {
            DateTime d = new DateTime(2008, 02, 28);
            DateTime e = DateTime.Now.AddHours(-2);
            string f = "Today @ " + e.ToShortTimeString();
            Assert.AreEqual("2008-02-28", d.ToFriendlyDate());
            Assert.AreEqual(f, e.ToFriendlyDate());

            // This only works if d has the same year as the current year as ToString("m") returns the full month name
            // and the date of the month when the date is in the current year. During the first few weeks of January this
            // date will be in the previous year and be printed out differently
            d = DateTime.Now.AddDays(-20);
            if(d.Year == DateTime.Now.Year)
                Assert.AreEqual(d.ToString("m"), d.ToFriendlyDate());
        }

        [Test]
        public void ToFriendlyDateNullable()
        {
            DateTime? d = null;
            Assert.AreEqual("", d.ToFriendlyDate());
        }

        [Test]
        public void ValidDate()
        {
            string s = "01/10/2011";
            string t = "Not a valid date";
            Assert.AreEqual(true, DateTimeExtensions.ValidDate(s));
            Assert.AreEqual(false, DateTimeExtensions.ValidDate(t));
        }

        [Test]
        public void TruncDate()
        {
            DateTime d = DateTime.Now;
            var year = d.Year;
            var month = d.Month;
            var day = d.Day;
            var hour = d.Hour;
            var minute = d.Minute;
            var sec = 0;
            DateTime e = new DateTime(year, month, day, hour, minute, sec);
            Assert.AreEqual(e, d.TruncSeconds());
        }

        [Test]
        public void StartOfMonth()
        {
            var start = new DateTime(2011, 1, 1);
            Assert.AreEqual(start, start.StartOfMonth());
            Assert.AreEqual(start, start.AddDays(2).StartOfMonth());
            Assert.AreEqual(start, start.AddDays(28).StartOfMonth());
            Assert.AreEqual(start, new DateTime(2011, 1, 31).StartOfMonth());

            Assert.AreNotEqual(start, start.AddMonths(1).StartOfMonth());
            Assert.AreNotEqual(start, start.AddMonths(-1).StartOfMonth());
        }

        [Test]
        public void EndOfMonth()
        {
            var end = new DateTime(2011, 1, 31);
            Assert.AreEqual(end, end.AddDays(-2).EndOfMonth());
            Assert.AreEqual(end, new DateTime(2011, 1, 1).EndOfMonth());
            Assert.AreEqual(end, end.EndOfMonth());

            Assert.AreEqual(new DateTime(2012, 2, 29), new DateTime(2012, 2, 1).EndOfMonth());

            Assert.AreNotEqual(end, end.AddMonths(1).EndOfMonth());
            Assert.AreNotEqual(end, end.AddMonths(-1).EndOfMonth());
        }

        [Test]
        public void ToUtc()
        {
            DateTime d = new DateTime(2011, 01, 10, 10, 10, 10);
            var e = new DateTime(2011, 01, 10, 10, 10, 10, DateTimeKind.Utc);
            Assert.AreEqual(e, d.ToUtc());
        }

        [Test]
        public void FuzzyName()
        {
            DateTime d = DateTime.Now;
            var time = d.TimeOfDay;
            var militaryHours = d.ToString("HH");
            if(militaryHours[0] == '0')
            {
                militaryHours = militaryHours.Substring(1) + " Hrs";
            }
            else
            {
                militaryHours = militaryHours + " Hrs";
            }
            var twoDaysAgo = d.AddDays(-2);
            var fortyEightHours = d.Subtract(twoDaysAgo);
            Assert.AreEqual(militaryHours, time.FuzzyName());
            Assert.AreEqual("48 Hrs", fortyEightHours.FuzzyName());


        }

        [Test]
        public void Age()
        {
            DateTime d = new DateTime(2011, 01, 10, 10, 10, 10);
            var age = DateTime.Now - d;
            // this test can't take longer than 1 second or it will fail
            var minSpeed = new TimeSpan(0, 0, 1);
            // Warning, this may randomly fail since the call to d.Age()
            // happens separately from the Assert.AreEqual()
            Assert.LessOrEqual(age, d.Age());
            Assert.GreaterOrEqual(age + minSpeed, d.Age());
        }

        [Test]
        public void AddMonthsMatchDay()
        {
            var jan = new DateTime(2008, 1, 31);

            var feb = jan.AddMonthsMatchDay(1, 31);
            var march = feb.AddMonthsMatchDay(1, 31);
            var april = march.AddMonthsMatchDay(1, 31);

            Assert.AreEqual(29, feb.Day, "Failed to pick the max day of the month for February 2008");
            Assert.AreEqual(31, march.Day, "Failed to pick the match day for March 2008");
            Assert.AreEqual(30, april.Day, "Failed to pick the max day of the month for April 2008");
        }

        [Test]
        public void IsEnteredTimeWithTimeRange()
        {
            TimeSpan entertedTime1 = "21:00".ConvertTo<DateTime>().TimeOfDay;
            TimeSpan entertedTime2 = "9:00".ConvertTo<DateTime>().TimeOfDay;
            TimeSpan startTime = "8:00AM".ConvertTo<DateTime>().TimeOfDay;
            TimeSpan endTime = "5:00PM".ConvertTo<DateTime>().TimeOfDay;
            Assert.IsFalse(DateTimeExtensions.IsEnteredTimeWithTimeRange(entertedTime1, startTime, endTime), entertedTime1 + " exceeds the valid range");
            Assert.IsTrue(DateTimeExtensions.IsEnteredTimeWithTimeRange(entertedTime2, startTime, endTime), entertedTime1 + " exceeds the valid range");

        }

        [Test]
        public void IsValidTime()
        {
            string time = "21:00";
            string time2 = "2100";
            string time3 = "00:00";
            string time4 = "8:00AM";
            string noTime = "Not a valid time";
            Assert.IsTrue(DateTimeExtensions.IsValidTime(time), time + " should be valid but is not valid.");
            Assert.IsTrue(DateTimeExtensions.IsValidTime(time3), time3 + " should be valid but is not valid.");
            Assert.IsFalse(DateTimeExtensions.IsValidTime(time4), time4 + " cannot append AM or PM to time.");

            Assert.IsFalse(DateTimeExtensions.IsValidTime(time2), time2 + " Time enter is not in correct format");

            Assert.IsFalse(DateTimeExtensions.IsValidTime(noTime), noTime + " should be a string with digits but it is not.");
        }
    }
}
