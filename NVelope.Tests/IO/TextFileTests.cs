using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.IO;
using System.IO;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class TextFileTests
    {
        private const string file = "spit.txt";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        [Test]
        public void SpitAndSlurp()
        {
            var data = "boosums";
            TextFile.Spit(file, data);

            var read = TextFile.Slurp(file);
            Assert.AreEqual(data, read);
        }

        [Test]
        public void SlurpNonExistantFile()
        {
            var read = TextFile.Slurp("asdfasfdasdfasdfdsafafdssafd");
            Assert.AreEqual("", read);
        }

        [Test]
        public void SpitExistingFile()
        {
            TextFile.Spit(file, "abc");
            // Now, spit again - it should overwrite the existing file
            TextFile.Spit(file, "def");
            var read = TextFile.Slurp(file);
            Assert.AreEqual("def", read);
        }

        [Test]
        public void SpitDoesntDuplicateNewlines()
        {
            TextFile.Spit(file, "abc/n/n");
            var read = TextFile.Slurp(file);
            Assert.AreEqual("abc/n/n", read);
        }

    }
}
