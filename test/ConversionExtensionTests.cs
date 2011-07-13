using System;
using System.Data;
using System.Xml;
using Nvelope.Exceptions;
using Nvelope.Reflection;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class ConversionExtensionTests
    {
        [Test]
        public void Print()
        {
            Assert.AreEqual("ab", "ab".Print());
            Assert.AreEqual("23", 23.Print());
            Assert.AreEqual("1000", 1000m.Print());
            Assert.AreEqual("100", 100.000m.Print());
            Assert.AreEqual("0.05", 0.0500m.Print());
        }

        [Test]
        public void Realize()
        {
            object o = 5;
            Assert.AreEqual(5, o.Realize());
            o = new Func<object>(() => 7);
            Assert.AreNotEqual(7, o);
            Assert.AreEqual(7, o.Realize());
        }

        [Test]
        public void IntConversion()
        {
            Assert.AreEqual(5, "5".ConvertTo<int>());
            Assert.AreEqual(5, "5".ConvertTo<int?>());
            Assert.IsTrue("".CanConvertTo<int?>());
            Assert.IsNull("".ConvertTo<int?>());
            Assert.IsFalse("adsfds".CanConvertTo<int>());
            Assert.IsFalse("adsfas".CanConvertTo<int?>());
        }

        [Test]
        public void DateTimeConversion()
        {
            // Convience function - this just converts a string to a datetime
            // (using the m ConvertTo method we're testing), and then prints it out
            // This saves me haveing to type .ConvertTo{DateTime}().ToIsoDate() in
            // all the Assert statements
            Func<string, string> Fn = s => s.ConvertTo<DateTime>().ToIsoDateTime();

            var time = DateTime.Now;
            Assert.AreEqual(time.ToIsoDateTime(), Fn(time.ToIsoDateTime()));
            Assert.AreEqual("1982-06-25 11:12:13", Fn("25/06/1982 11:12:13 AM"));
            Assert.AreEqual("1982-06-25 11:12:13", Fn("25\\06\\1982 11:12:13 AM"));
            Assert.AreEqual("1982-06-25 11:12:13", Fn("25-06-1982 11:12:13 AM"));
            Assert.AreEqual("1982-06-25 23:12:13", Fn("25-06-1982 11:12:13 PM"));
            Assert.AreEqual("1982-06-25 00:00:00", Fn("25-06-1982"));
            Assert.AreEqual("1982-06-25 00:00:00", Fn("25/06/1982"));
            Assert.AreEqual("1982-06-05 00:00:00", Fn("5/6/1982"));
            Assert.AreEqual("1982-06-05 00:00:00", Fn("5-6-1982"));
            
            Assert.AreEqual("2006-07-01 11:09:23", Fn("2006-07-01 11:09:23"));
            
            // two-digit years should be in this decade, the next one, or in the last 8
            Assert.AreEqual("1982-06-05 00:00:00", Fn("5/6/82"));
            Assert.AreEqual("2002-06-05 00:00:00", Fn("5-6-02"));
            Assert.AreEqual("1982-06-05 00:00:00", Fn("5\\6\\82"));
            Assert.AreEqual("2002-06-05 11:12:13", Fn("5/6/02 11:12:13"));
            // next decade
            Assert.AreEqual("2017-06-05 00:00:00", Fn("5/6/17"));
            Assert.AreEqual("2017-06-05 23:12:00", Fn("5-6-17 11:12PM"));
            if (DateTime.Now.Year < 2020) // this test won't work after this - it will assume 2031
            {
                Assert.AreEqual("1931-06-05 00:00:00", Fn("5/6/31"));
                Assert.AreEqual("1931-06-05 11:12:00", Fn("5/6/31 11:12"));
            }
        }

        [Test]
        public void ConvertsMonth()
        {
            Assert.AreEqual(Month.March, "March".ConvertTo<Month>());
            Assert.AreEqual(Month.March, "Mar".ConvertTo<Month>());
            Assert.AreEqual(Month.March, 3.ConvertTo<Month>());
        }

        [Test]
        public void ConvertsNoonProperly()
        {
            Assert.AreEqual(new DateTime(1985, 9, 23, 12, 00, 00), ConversionExtensions.ToDateTimeNullable("9/23/1985 12:00:00 PM"));       
        }

        [Test]
        public void MakeFourDigitYear()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { ConversionExtensions.MakeFourDigitYear(200); });
            Assert.Throws<ArgumentOutOfRangeException>(
                () => { ConversionExtensions.MakeFourDigitYear(-12); });

            Assert.AreEqual(1988, ConversionExtensions.MakeFourDigitYear(88));
            Assert.AreEqual(1994, ConversionExtensions.MakeFourDigitYear(94));
            Assert.AreEqual(2002, ConversionExtensions.MakeFourDigitYear(2));
            Assert.AreEqual(2013, ConversionExtensions.MakeFourDigitYear(13));
            if (DateTime.Now.Year < 2020)// this test won't work after this - it will assume 2031
                Assert.AreEqual(1931, ConversionExtensions.MakeFourDigitYear(31));
        }

        [Test]
        public void SetFrom()
        {
            var data = new Data() { IntField = 1, StrField = "one" };
            var dict = data._AsDictionary();

            dict["IntField"] = 11;
            // Should change IntField to 11
            data._SetFrom(dict);
            Assert.AreEqual("([IntField,11],[StrField,one])", data._Inspect());
        }

        [Test]
        public void AsDictionary()
        {
            var data = new Data() { IntField = 1, StrField = "a" };
            
            Assert.AreEqual("([IntField,1],[StrField,a])", data._Inspect());
        }

        [Test]
        public void SetFrom_CaseInsensitive()
        {
            var data = new Data() { IntField = 1, StrField = "one" };
            var lowerData = new LowerData() { intField = 2, strField = "two" };

            // This shouldn't work, because the default is to have names match exactly
            data._SetFrom(lowerData, true);
            Assert.AreEqual("([IntField,1],[StrField,one])", data._Inspect());

            // Should change IntField to 2
            data._SetFrom(lowerData, false);
            Assert.AreEqual("([IntField,2],[StrField,two])", data._Inspect());

        }

        [Test]
        public void ConvertTo_Uses_Constructor()
        {
            var s = "foosums";
            // ConvertTo should find the constructor that takes a string
            // as an argument and use that to construct the thing
            var res = s.ConvertTo<StringAlt>();

            Assert.AreEqual(s, res.Contents);
        }

        [Test]
        public void Null_to_nullable_type()
        {
            object o = null;
            Assert.AreEqual(null, o.ConvertTo<int?>());
            Assert.AreEqual(null, o.ConvertTo<DateTime?>());
        }

        [Test]
        public void Int_to_Enum()
        {
            Assert.AreEqual(TestEnum.One, 1.ConvertTo<TestEnum>());
        }

        [Test]
        public void Enum_to_int()
        {
            Assert.AreEqual(1, TestEnum.One.ConvertTo<int>());
        }

        [Test]
        public void NonGenericCanConvertToWorks()
        {
            Assert.True("a".CanConvertTo<string>());
            Assert.True("a".CanConvertTo(typeof(string)));
        }

        [Test]
        public void ScientificNotationToDecimal()
        {
            Assert.AreEqual(0.0375m, "3.75E-2".ConvertTo<decimal>());
        }

        [Test]
        public void ToBoolFriendly()
        {
            Assert.AreEqual(true, "y".ToBoolFriendly());
            Assert.AreEqual(false, "f".ToBoolFriendly());
            
           var ex = Assert.Throws<ConversionException>(() => "zip".ToBoolFriendly());
           Assert.AreEqual("Could not convert value 'zip' to a Boolean", ex.Message);
        }

        [Test]
        public void GetException()
        {
            DataTable dt = new DataTable();
            var ex = Assert.Throws<FieldNotFoundException>(()=>dt.GetFieldValue("fieldName"));
            Assert.AreEqual("Could not find a field called fieldName on the object", ex.Message);
        }

        [Test]
        public void Get()
        {
            DataTable table = new DataTable(); // Create DataTable
            table.TableName = "TestTable";
            Assert.AreEqual("TestTable", table.GetFieldValue("TableName"));
        }

        [Test]
        public void GetT()
        {
            DataTable table = new DataTable(); // Create DataTable
            table.TableName = "123";
            Assert.AreEqual(123, table.GetFieldValue<int>("TableName"));
        }

        [Test]
        public void IsXml()
        {
            string s = "<note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>";
            XmlDocument doc = new XmlDocument();
            Assert.AreEqual(true, s.IsXml(out doc));
        }

        [Test]
        public void ToXml()
        {
            string s = "<note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(("<note><to>Tove</to><from>Jani</from><heading>Reminder</heading><body>Don't forget me this weekend!</body></note>")); 

            Assert.AreEqual(doc, s.ToXml());
        }

        [Test]
        public void ToMonth()
        {
            string jan = "jan";
            string feb = "feb";
            Assert.AreEqual(Month.January, jan.ToMonth());
            Assert.AreEqual(Month.February, feb.ToMonth());
        }

        [Test]
        public void ToMonthException()
        {
            string s = "This is not a month";
            var ex = Assert.Throws<ConversionException>(() => s.ToMonth());
            Assert.AreEqual("Could not convert 'This is not a month' to Month", ex.Message);
        }
        [Test]
        public void ToDateTimeFriendly()
        {
            var s = "January 10, 2010";
            DateTime d = new DateTime(2010, 01, 10);
            Assert.AreEqual(d, s.ToDateTimeFriendly());
        }

        [Test]
        public void ToDateTimeFriendlyException()
        {
            var s = "This is not a date";
            var ex = Assert.Throws<ConversionException>(() => s.ToDateTimeFriendly());
            Assert.AreEqual("Could not convert value \"This is not a date\" to DateTime", ex.Message);
        }

        [Test]
        public void IsBool()
        {
            string s = "yes";
            string t = "no";
            string u = "maybe";
            Assert.AreEqual(true, s.IsBool());
            Assert.AreEqual(true, t.IsBool());
            Assert.AreEqual(false, u.IsBool());
        }
        [Test]
        public void ConvertAs()
        {
            Assert.AreEqual(-1, "-1".ConvertAs<int>());
            Assert.AreEqual(0, "0".ConvertAs<int>());
            Assert.AreEqual(1, "1".ConvertAs<int>());
            Assert.AreEqual(null, "b".ConvertAs<int>());
            Assert.AreEqual(1, (1.0).ConvertAs<int>());
            Assert.AreEqual(1, (1.1).ConvertAs<int>());
        }


        #region Data classes
        private class LowerData
        {
            public int intField;
            public string strField;
        }
        private class Data
        {
            public int IntField;
            public string StrField;
        }
        public class StringAlt
        {
            public StringAlt(string s)
            {
                Contents = s;
            }

            public string Contents;
        }

        public enum TestEnum { One = 1, Two = 2 }
        #endregion

    }
}
