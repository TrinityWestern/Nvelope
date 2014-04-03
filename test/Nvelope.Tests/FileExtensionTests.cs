using System.IO;
using NUnit.Framework;

namespace Nvelope.Tests
{
    [TestFixture]
    public class FileExtensionTests
    {
        [Test]
        public void MD5()
        {
            var f = new FileInfo("text.txt");
            Assert.AreEqual("E2D2638AEBBA25CA03DEC5FA49C943CB", f.MD5());
        }
    }
}
