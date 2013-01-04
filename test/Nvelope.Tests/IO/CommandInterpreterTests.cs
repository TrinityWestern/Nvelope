using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Nvelope.IO;
using Nvelope;
using Nvelope.Reflection;

namespace Nvelope.Tests.IO
{
    [TestFixture]
    public class CommandInterpreterTests
    {
        [Test]
        public void ParseErrors()
        {
            var interp = new CommandInterpreter(new StringWriter(), new StringWriter());
            interp.AddCommand<int, int, int>("plus", (i, j) => i + j);

            Assert.Throws<ParseException>(() => interp.Eval("plus 2 abc"));
        }

        [Test]
        public void Eval()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var args = new CommandArg() { Type = typeof(int) }.And(new CommandArg() { Type = typeof(int) });
            interp.AddCommand("plus", args, (o, e, c) => o.WriteLine(c["0"].ConvertTo<int>() + c["1"].ConvertTo<int>()));

            interp.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromSignature()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            interp.AddCommand<int, int, int>("plus", (i, j) => i + j);

            interp.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromMethodInfo()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var obj = new CommandObj();
            interp.AddCommand("Plus", obj);

            interp.Eval("Plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromAsyncMethodInfo()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var obj = new CommandObj();
            interp.AddCommand("plus", obj, "AsyncPlus");

            interp.Eval("plus 2 2");
            Assert.AreEqual("4", io.ToString());
        }

        [Test]
        public void EvalFromAction()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var obj = new CommandObj();
            interp.AddCommand("plus", new Action<TextWriter, int, int>(obj.AsyncPlus));

            interp.Eval("plus 2 2");
            Assert.AreEqual("4", io.ToString());
        }

        [Test]
        public void EvalArgless()
        {
            var interp = new CommandInterpreter();
            int ran = 0;
            interp.AddCommand("run", new Action(() => ran++));

            interp.Eval("run");
            Assert.AreEqual(1, ran);
        }


        [Test]
        public void EvalAsyncArgless()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var obj = new CommandObj();
            interp.AddCommand("say", new Action<TextWriter>(obj.AsyncArgless));

            interp.Eval("say");
            Assert.AreEqual("hi", io.ToString());
        }

        [Test]
        public void EvalWithFlag()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var obj = new CommandObj();
            interp.AddCommand("plus", obj, "PlusFlag");

            interp.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());

            interp.Output = new StringWriter();
            interp.Eval("plus 2 2 --invert");
            Assert.AreEqual("-4\r\n", interp.Output.ToString());
        }

        [Test]
        public void EvalWithSwitch()
        {
            var interp = new CommandInterpreter(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            interp.AddCommand("plus", obj, "PlusSwitch");

            interp.Eval("plus 2 2");
            Assert.AreEqual("5\r\n", interp.Output.ToString());

            interp.Output = new StringWriter();
            interp.Eval("plus 2 2 -c 2");
            Assert.AreEqual("6\r\n", interp.Output.ToString());
        }

        [Test]
        public void EvalWithNullDefaults()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var commandObj = new CommandObj();
            interp.AddCommand("join", commandObj, "Join");

            interp.Eval("join");
            Assert.AreEqual("ab\r\n", io.ToString());
        }

        [Test]
        public void EvalWithStringListArg()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io, new CommandParser(), new CommandObj());
            interp.Eval("ListArg 1 2 --c test,more-text");
            Assert.AreEqual("(test,more-text)\r\n", io.ToString());
        }

        [Test]
        public void EvalWithQuotedCommandName()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io, new CommandParser(), new CommandObj());
            interp.Eval("\"Plus\" 1 2");
            Assert.AreEqual("3\r\n", io.ToString());
        }
    }

    public class CommandObj
    {
        public int Plus(int a, int b)
        {
            return a + b;
        }

        public void AsyncPlus(TextWriter output, int i, int j)
        {
            output.Write(i + j);
        }

        public int PlusFlag(int a, int b, bool invert = false)
        {
            return (a + b) * (invert ? -1 : 1);
        }

        public int PlusSwitch(int a, int b, int c = 1)
        {
            return a + b + c;
        }

        public void AsyncArgless(TextWriter output)
        {
            output.Write("hi");
        }

        public string Join(string a = null, string b = null)
        {
            a = a ?? "a";
            b = b ?? "b";
            return a + b;
        }

        public string ListArg(int a, int b, IEnumerable<string> c = null)
        {
            return c.Print();
        }
    }
}
