using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void ToPrettyTime()
        {
            DateTime d = new DateTime(2011, 01, 10, 10, 10, 10);
            Assert.AreEqual("10:10 am", d.ToPrettyTime());
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
            if (militaryHours[0] == '0')
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
        public void ToIntTime()
        {
            DateTime d = new DateTime(2011, 01, 10, 23, 10, 10);
            Assert.AreEqual(2310, d.ToIntTime());
        }

        [Test]
        public void ToJulianDate()
        {
            DateTime d = new DateTime(2011, 11, 10, 23, 10, 10);
            Assert.AreEqual(2011314, d.ToJulianDate());
        }
    }
}
