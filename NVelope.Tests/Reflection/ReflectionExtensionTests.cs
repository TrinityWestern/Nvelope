using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml;
using Nvelope.Exceptions;
using System.Data;
using Nvelope.Reflection;
using System.Drawing;
using System.Reflection;

namespace Nvelope.Reflection.Tests
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
            var declare = members.Declarations();
            Assert.AreEqual(typeof(Color), declare["Paint"]);
            var allmembers = dh._GetMembers(MemberTypes.All);
            Assert.Throws<ArgumentException>(() => allmembers.Declarations());
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
