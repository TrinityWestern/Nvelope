using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Nvelope.Reflection;

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
            Assert.Throws<ArgumentException>(() => allmembers.FieldDeclarations());
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
