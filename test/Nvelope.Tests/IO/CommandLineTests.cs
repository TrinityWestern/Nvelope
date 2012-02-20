using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.IO;
using Nvelope;
using Nvelope.Reflection;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class CommandLineTests
    {
        [Test]
        public void Parse()
        {
            var res = CommandLine.Parse("a b c");
            Assert.AreEqual("(a,b,c)", res.Args.Print());
        }

        [Test]
        public void ParseQuotedArgs()
        {
            var res = CommandLine.Parse("a \"arg with space\" c");
            Assert.AreEqual("(a,arg with space,c)", res.Args.Print());
        }

        [Test]
        public void ParseTypedArgs()
        {
            var res = CommandLine.Parse("false 42 abc", typeof(bool).And(typeof(int)));
            Assert.AreEqual("(False,42,abc)", res.Args.Print());
            Assert.AreEqual(typeof(bool), res.Args.First().GetType());
            Assert.AreEqual(typeof(int), res.Args.Second().GetType());
            Assert.AreEqual(typeof(string), res.Args.Third().GetType());
        }

        [Test]
        public void ParseFlags()
        {
            var res = CommandLine.Parse("-flag1 --flag2", flags: "flag1".And("flag2"));
            Assert.AreEqual("(flag1,flag2)", res.Flags.Print());
        }

        [Test]
        public void ParseSwitches()
        {
            var switches = new { s1 = "", s2 = false }._FieldTypes();
            var res = CommandLine.Parse("-s1 val1 --s2 true", switches: switches);
            Assert.AreEqual("([s1,val1],[s2,True])", res.Switches.Print());
        }

        [Test]
        public void ParseTypedSwitches()
        {
            var switches = new { s1 = typeof(string), s2 = typeof(bool), s3 = typeof(int) }._AsDictionary().SelectVals(o => o as Type);
            var res = CommandLine.Parse("-s1 a -s2 false --s3 42", switches: switches);
            Assert.AreEqual("([s1,a],[s2,False],[s3,42])", res.Switches.Print());
            Assert.AreEqual(res.Switches["s2"].GetType(), typeof(bool));
            Assert.AreEqual(res.Switches["s3"].GetType(), typeof(int));
        }

        [Test]
        public void ParseQuotedSwitches()
        {
            var switches = new { s1 = "", s2 = "" }._FieldTypes();
            var res = CommandLine.Parse("-s1 abc -s2 \"switch with space\"", switches: switches);
            Assert.AreEqual("([s1,abc],[s2,switch with space])", res.Switches.Print());
        }

        [Test]
        public void ParseArgsAndFlags()
        {
            var res = CommandLine.Parse("a b --f1 c --f2", flags: "f1".And("f2"));
            Assert.AreEqual("(a,b,c)", res.Args.Print());
            Assert.AreEqual("(f1,f2)", res.Flags.Print());
        }

        [Test]
        public void ParseArgsSwitchesFlags()
        {
            var switches = new { s1 = "", s2 = false }._FieldTypes();
            var res = CommandLine.Parse("abc --f1 --s2 true 42 -s1 \"some text\" -f2",
                typeof(string).And(typeof(int)),
                switches,
                "f1".And("f2"));

            Assert.AreEqual("(abc,42)", res.Args.Print());
            Assert.AreEqual("([s1,some text],[s2,True])", res.Switches.Print());
            Assert.AreEqual("(f1,f2)", res.Flags.Print());
        }

    }
}
