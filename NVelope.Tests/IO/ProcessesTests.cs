using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.IO;
using Nvelope;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class ProcessesTests
    {
        [Test]
        public void Execute()
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            var started = DateTime.Now;
            var res = Processes.Execute(Folder.ComposePath(windir, "System32", "ipconfig.exe"), "");
            var ended = DateTime.Now;
            Assert.Greater(res.Length, 10);
            Assert.True(res.ContainsAll("Windows", "Configuration"));
            Assert.IsTrue(ended.Subtract(started).Seconds < 10, "Process took too long to execute - it probably timed out");
        }
    }
}
