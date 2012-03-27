using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nvelope.IO;
using Nvelope;
using Nvelope.Reflection;
using Nvelope.Reading;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        public void Parse()
        {
            var res = new CommandParser().Parse("a b c");
            Assert.AreEqual("([0,a],[1,b],[2,c])", res.Print());
        }

        [Test]
        public void ParseQuotedArgs()
        {
            var res = new CommandParser().Parse("a \"arg with space\" c");
            Assert.AreEqual("([0,a],[1,arg with space],[2,c])", res.Print());
        }

        [Test]
        public void ParseTypedArgs()
        {
            var args = new CommandArg() { Name = "1", Type = typeof(bool), IsOptional = false }
                .And(new CommandArg() { Name = "2", Type = typeof(int), IsOptional = false })
                .And(new CommandArg() { Name = "3", Type = typeof(string), IsOptional = false });

            var res = new CommandParser().Parse("false 42 abc", args);
            Assert.AreEqual("([1,False],[2,42],[3,abc])", res.Print());
            Assert.AreEqual(typeof(bool), res["1"].GetType());
            Assert.AreEqual(typeof(int), res["2"].GetType());
            Assert.AreEqual(typeof(string), res["3"].GetType());
        }

        [Test]
        public void ParseTwoInts()
        {
            var args = new CommandArg() { Name = "a", Type = typeof(int), IsOptional = false }
                .And(new CommandArg() { Name = "b", Type = typeof(int), IsOptional = false });

            var res = new CommandParser().Parse("2 2", args);
            Assert.AreEqual("([a,2],[b,2])", res.Print());
        }


        [Test]
        public void ParseFlags()
        {
            var args = new CommandArg() { Name = "flag1", Type = typeof(bool), IsOptional = true }
                .And(new CommandArg() { Name = "flag2", Type = typeof(bool), IsOptional = true });

            var res = new CommandParser().Parse("-flag1 --flag2", args);
            Assert.AreEqual("([flag1,True],[flag2,True])", res.Print());
        }

        [Test]
        public void ParseSwitches()
        {
            var args = new CommandArg() { Name = "s1", Type = typeof(string), IsOptional = true }
                .And(new CommandArg() { Name = "s2", Type = typeof(bool), IsOptional = true });

            var res = new CommandParser().Parse("-s1 val1 --s2 true", args);
            Assert.AreEqual("([s1,val1],[s2,True])", res.Print());
        }

        [Test]
        public void ParseQuotedSwitches()
        {
            var args = new CommandArg() { Name = "s1", Type = typeof(string), IsOptional = true }
                .And(new CommandArg() { Name = "s2", Type = typeof(string), IsOptional = true });

            var res = new CommandParser().Parse("-s1 abc -s2 \"switch with space\"", args);
            Assert.AreEqual("([s1,abc],[s2,switch with space])", res.Print());
        }

        [Test]
        public void ParseArgsAndFlags()
        {
            var args = new CommandArg() { Name = "f1", Type = typeof(string), IsOptional = true }
                .And(new CommandArg() { Name = "f2", Type = typeof(bool), IsOptional = true })
                .And(new CommandArg() { Type = typeof(string), IsOptional = false })
                .And(new CommandArg() { Type = typeof(string), IsOptional = false });

            var res = new CommandParser().Parse("a b --f1 c --f2", args);
            Assert.AreEqual("([0,a],[1,b],[f1,c],[f2,True])", res.Print());
        }

        [Test]
        public void ExtraParamsThrowError()
        {
            var args = new CommandArg() { Type = typeof(string) }.List();
            Assert.Throws<ParseException>(() => new CommandParser().Parse("a b", args));
        }


        [Test]
        public void InterspersedFlagsDontClobberArgs()
        {
            var args = new CommandArg() { Name = "f1", Type = typeof(bool), IsOptional = true }
                .And(new CommandArg() { Name = "f2", Type = typeof(bool), IsOptional = true })
                .And(new CommandArg() { Type = typeof(string), IsOptional = false })
                .And(new CommandArg() { Type = typeof(string), IsOptional = false });

            var res = new CommandParser().Parse("a --f1 b --f2", args);
            Assert.AreEqual("([0,a],[1,b],[f1,True],[f2,True])", res.Print());
        }

        [Test]
        public void BadTypeThrowsParseError()
        {
            var args = new CommandArg() { Name = "f1", Type = typeof(bool) }.List();

            Assert.Throws<ParseException>(() => new CommandParser().Parse("--f1 abcd", args));
        }

        [Test]
        public void ParseArgsSwitchesFlags()
        {
            var args = new CommandArg() { Type = typeof(string)}
                .And(new CommandArg() { Type = typeof(int)})
                .And(new CommandArg() { Name = "f1", Type = typeof(bool), IsOptional = true })
                .And(new CommandArg() { Name = "f2", Type = typeof(bool), IsOptional = true })
                .And(new CommandArg() { Name = "s1", Type = typeof(string), IsOptional = true})
                .And(new CommandArg() { Name = "s2", Type = typeof(bool), IsOptional = true});

            var res = new CommandParser().Parse("abc --f1 --s2 false 42 -s1 \"some text\" -f2", args);

            Assert.AreEqual("([0,abc],[1,42],[f1,True],[f2,True],[s1,some text],[s2,False])", res.Print());
        }
    }

    [TestFixture(Description="You should only need these tests if the CommandLineTests don't pass - they test specific sub-parts of CommandLine")]
    public class CommandLineTestsDetailed
    {
        [Test]
        public void ParseArgs()
        {
            var res = new CommandParser().ParseArgs("a".And("b").And("c"), new string[]{});
            Assert.AreEqual(3, res.Count());
        }

        [Test]
        public void AssignArgs()
        {
            var parsed = new KeyValuePair<string,string>(null, "a")
                .And(new KeyValuePair<string, string>(null, "b"));

            var res = new CommandParser().AssignArgs(parsed, new CommandArg[]{});

            Assert.AreEqual("([0, a],[1, b])", res.Print());
        }

        [Test]
        public void TrimQuotesFromQuotedStrings()
        {
            var parsed = new CommandParser().Lex("a \"arg with space\" c");
            Assert.AreEqual("(a,arg with space,c)", parsed.Print());
        }

        [Test]
        public void ParseFlags()
        {  
            var lexed = Read.List("(a,--f1,b,--f2)");
            var parsed = new CommandParser().ParseArgs(lexed, "f1".And("f2"));
            Assert.AreEqual("([, a],[f1, ],[, b],[f2, ])", parsed.Print());
        }
    }
}
