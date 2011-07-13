using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        [Test]
        public void Eq()
        {
            object a = 1;
            object b = 1;
            Assert.False(a == b);
            Assert.True(a.Eq(b));

            b = null;
            Assert.False(a.Eq(b));
            Assert.False(b.Eq(a));
            a = null;
            Assert.True(a.Eq(b));
            Assert.True(b.Eq(a));
        }
               
        [Test]
        public void PrintDecimalWorksPolymorphically()
        {
            var dec = 1.0000m;
            object obj = dec;

            Assert.AreEqual("1", dec.Print());
            // We have a variable of type object, but it should
            // still call the decimal version of print, not the
            // generic object ToString version. If we call decimal.ToString(),
            // we'll get "1.0000"
            Assert.AreNotEqual("1.0000", obj.Print(), 
                "Print was not called polymorphically. You probably changed the object.Print() extension " + 
                " method in a way you shouldn't have");
            Assert.AreEqual("1", obj.Print());
        }
    }
}
