﻿using System;
using NUnit.Framework;
using Nvelope.Reading;
using Nvelope.Reflection;

namespace Nvelope.Tests.Reflection
{
    [TestFixture]
    public class SetTests
    {
        [Test]
        public void SerializeUseCase()
        {
            var a = new Foosums() { Date = new DateTime(2011, 8,2,14,1,22), Value = "foobar" };

            var str = a._Inspect();

            var b = new Foosums();
            var dict = Read.Dict(str);
            b._SetFrom(dict);

            Assert.AreEqual(a.Id, b.Id);
            Assert.AreEqual(a.Date, b.Date);
            Assert.AreEqual(a.Value, b.Value);

        }

        class Foosums
        {
            public int? Id;
            public DateTime Date;
            public string Value;
        }
    }
}