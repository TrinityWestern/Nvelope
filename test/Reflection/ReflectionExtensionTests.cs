using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Nvelope.Reflection;
using System.Collections.Generic;

namespace Nvelope.Tests.Reflection
{
    [TestFixture]
    public class ReflectionExtensionTests
    {
        [Test]
        public void GetMembers()
        {
            DataTable dt = null;
            Assert.IsFalse(dt._GetMembers().Any());

            var dh = new DollHouse();
            var members = dh._GetMembers();
            Assert.AreEqual(4, members.Count());

            var attr = members.FilterAttributeType<DummyAttribute>();
            Assert.AreEqual(1, attr.Count());

            var ro = members.RemoveReadOnly();
            Assert.IsFalse(ro.Names().Contains("Size"));
            Assert.IsTrue(ro.Names().Contains("Rooms"));
            Assert.IsTrue(ro.Names().Contains("Paint"));
        }

        [Test]
        public void Declarations()
        {
            var dh = new DollHouse();
            var members = dh._GetMembers();
            var declare = members.FieldDeclarations();
            Assert.AreEqual(typeof(Color), declare["Paint"]);
            var allmembers = dh._GetMembers(MemberTypes.All);
            var fields = allmembers.FieldDeclarations();
            Assert.AreEqual("([Paint,System.Drawing.Color],[Rooms,System.String[]],[Size,System.Int32],[Spam,System.String])",
                fields.Print());
        }

        [Test]
        public void GetNonGenericType()
        {
            Type i = typeof(int);
            Type inull = typeof(int?);
            Assert.AreEqual(inull.GetNonGenericType(), i);
        }

        [Test]
        public void PlayWithDollHouse()
        {
            var dh = new DollHouse();
            Assert.AreEqual(2, dh.Size);
            Assert.AreEqual("Eggs", dh.Spam);
            Assert.AreEqual(Color.White, dh.Paint);
            dh.Paint = Color.Black;
            Assert.AreEqual(Color.Black, dh.Paint);

        }

        [Test]
        public void _Inspect()
        {
            var data = new { IntField = 1, StrField = "a" };
            Assert.AreEqual("([IntField,1],[StrField,a])", data._Inspect());
        }

        [Test]
        public void _AsDictionary()
        {
            var data = new { IntField = 1, StrField = "a" }._AsDictionary();
            Assert.True(data.ContainsKey("IntField"));
            Assert.True(data.ContainsKey("StrField"));
            Assert.AreEqual(data["IntField"], 1);
            Assert.AreEqual(data["StrField"], "a");
        }

        [Test]
        public void _AsDictionary_KeepsAsDictionary()
        {
            var data = new Dictionary<string, object>();
            data.Add("IntField", 1);
            data.Add("StrField", "a");
            var res = data._AsDictionary();
            // Make sure the compiler doesn't screw around with us and change this on us
            Assert.AreEqual(typeof(Dictionary<string, object>), res.GetType());
            Assert.AreEqual("([IntField,1],[StrField,a])", res.Print());
        }

        [Test]
        public void _AsDictionary_WithFields_KeepsAsDictionary()
        {
            var data = new Dictionary<string, object>();
            data.Add("IntField", 1);
            data.Add("StrField", "a");
            var res = data._AsDictionary("IntField".And("StrField"));
            // Make sure the compiler doesn't screw around with us and change this on us
            Assert.AreEqual(typeof(Dictionary<string, object>), res.GetType());
            Assert.AreEqual("([IntField,1],[StrField,a])", res.Print());
        }

        [Test]
        public void _AsDictionary_KeepsAsDictionaryPolymorphically()
        {
            var data = new Dictionary<string, object>();
            data.Add("IntField", 1);
            data.Add("StrField", "a");
            // If we assign the dictionary to an object, it should still call the
            // right version of _AsDictionary internally, even though the compiler
            // will call the object version instead of the Dictionary<> version
            object obj = data;
            var res = obj._AsDictionary();
            Assert.AreEqual("([IntField,1],[StrField,a])", res.Print());
            // If the above has Comparer... etc in it, that means that instead of
            // evaluating the thing as a Dictionary and passing it back, the _AsDictionary
            // function got the properties of the Dictionary instead
        }

        [Test]
        public void _AsDictionary_WithFields_KeepsAsDictionaryPolymorphically()
        {
            var data = new Dictionary<string, object>();
            data.Add("IntField", 1);
            data.Add("StrField", "a");
            // If we assign the dictionary to an object, it should still call the
            // right version of _AsDictionary internally, even though the compiler
            // will call the object version instead of the Dictionary<> version
            object obj = data;
            var res = obj._AsDictionary("IntField".And("StrField"));
            Assert.AreEqual("([IntField,1],[StrField,a])", res.Print());
            // If the above has Comparer... etc in it, that means that instead of
            // evaluating the thing as a Dictionary and passing it back, the _AsDictionary
            // function got the properties of the Dictionary instead
        }

        [Test]
        public void _AsDictionary_WithDictionaryReturnsCopy()
        {
            var data = new Dictionary<string, object>();
            data.Add("IntField", 1);
            data.Add("StrField", "a");
            var res = data._AsDictionary();
            Assert.False(data == res, "The two dictionaries shouldn't have been the same object");
        }

        [Test]
        public void _Diff()
        {
            var a = new { A = 1, B = 2, C = 3 };
            var b = new { A = 1, B = -2, C = 3 };
            
            var diff = a._Diff(b);
            Assert.AreEqual("([B,(2, -2)])", diff.Print());

            diff = a._Diff(b, "A", "B");
            Assert.AreEqual("([B,(2, -2)])", diff.Print());

            diff = a._Diff(b, "A", "C");
            Assert.AreEqual("()", diff.Print());

            var copyA = new { A = 1, B = 2, C = 3 };
            diff = a._Diff(copyA);
            Assert.AreEqual("()", diff.Print());
        }

        [Test]
        public void _Diff_MissingFields()
        {
            var a = new { A = 1 };
            var b = new { B = 2 };

            var diff = a._Diff(b);
            Assert.AreEqual("([A,(1, )],[B,(, 2)])", diff.Print());
        }
    }

    public class DollHouse
    {
        public string[] Rooms = new string[] {
            "Dinning Room",
            "Kitchen"
        };

        public int Size
        {
            get
            {
                return Rooms.Count();
            }
        }
        /// <summary>
        /// This is here to test FilterAttributeType
        /// </summary>
        [DummyAttribute]
        public string Spam
        {
            get
            {
                return "Eggs";
            }
        }

        private Color? _paint;

        public Color Paint
        {
            get {
                return this._paint ?? Color.White;
            }
            set {
                this._paint = value;
            }
        }
    }
    [AttributeUsage(AttributeTargets.All)]
    public class DummyAttribute : System.Attribute
    {
        public DummyAttribute() { }
    }
}
