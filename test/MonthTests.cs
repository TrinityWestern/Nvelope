using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class MonthTests
    {
        [Test]
        public void CastToInt()
        {
            Assert.AreEqual(1, (int)Month.January);            
        }

        [Test]
        public void ConvertToInt()
        {
            Assert.AreEqual(1, Month.January.ConvertTo<int>());
        }

        [Test]
        public void FromInt()
        {
            Assert.AreEqual(Month.January, 1.ConvertTo<Month>());            
        }
    }
}
