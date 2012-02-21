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
    public class CommandContextTests
    {
        [Test]
        public void DetermineSchemas()
        {
            Assert.Fail();
        }

        [Test]
        public void Eval()
        {
            var io = new StringWriter();
            var context = new CommandContext(io, io);
            var schema = new CommandSchema() { ArgTypes = typeof(int).And(typeof(int)).ToList() };
            context.AddCommand("plus", schema, (o, e, c) => o.WriteLine(c.Args[0].ConvertTo<int>() + c.Args[1].ConvertTo<int>()));
            
            context.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromSignature()
        {
            var io = new StringWriter();
            var context = new CommandContext(io, io);
            context.AddCommand<int,int,int>("plus", (i, j) => i + j);

            context.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromMethodInfo()
        {
            var io = new StringWriter();
            var context = new CommandContext(io, io);
            var obj = new CommandObj();
            context.AddCommand("Plus", obj);

            context.Eval("Plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromAsyncMethodInfo()
        {
            var io = new StringWriter();
            var context = new CommandContext(io, io);
            var obj = new CommandObj();
            context.AddCommand("plus", obj, "AsyncPlus");

            context.Eval("plus 2 2");
            Assert.AreEqual("4", io.ToString());
        }

        [Test]
        public void GetSchemaWithFlag()
        {
            var obj = new CommandObj();
            var mi = obj.GetType().GetMethod("PlusFlag");
            var schema = CommandContext.GetSchema(mi);

            Assert.AreEqual(2, schema.ArgTypes.Count());
            Assert.AreEqual(1, schema.FlagTypes.Count());
        }

        [Test]
        public void ParseCommand()
        {
            var context = new CommandContext(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            context.AddCommand("plus", obj, "PlusFlag");

            var command = context.ParseCommand("plus 2 2 --invert");
            Assert.AreEqual("(2,2)", command.Args.Print());
            Assert.AreEqual("(invert)", command.Flags.Print());
        }


        [Test]
        public void GetParameters()
        {
            var context = new CommandContext(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            context.AddCommand("plus", obj, "PlusFlag");
            var pi = obj.GetType().GetMethod("PlusFlag").GetParameters().OrderBy(p => p.Position);
            var schema = context.Commands.First().Item2;

            var command = context.ParseCommand("plus 2 2 --invert");
            var paras = context.GetParameters(schema, pi, command);
            Assert.AreEqual(3, paras.Count());
            Assert.AreEqual(2, paras.First());
            Assert.AreEqual(2, paras.Second());
            Assert.AreEqual(true, paras.Third());

            command = context.ParseCommand("plus 2 2");
            paras = context.GetParameters(schema, pi, command);
            Assert.AreEqual(3, paras.Count());
            Assert.AreEqual(false, paras.Third());
        }

        [Test]
        public void EvalWithFlag()
        {
            var io = new StringWriter();
            var context = new CommandContext(io, io);
            var obj = new CommandObj();
            context.AddCommand("plus", obj, "PlusFlag");

            context.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());

            context.Output = new StringWriter();
            context.Eval("plus 2 2 --invert");
            Assert.AreEqual("-4\r\n", context.Output.ToString());
        }

        [Test]
        public void EvalWithSwitch()
        {
            var context = new CommandContext(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            context.AddCommand("plus", obj, "PlusSwitch");

            context.Eval("plus 2 2");
            Assert.AreEqual("5\r\n", context.Output.ToString());

            context.Output = new StringWriter();
            context.Eval("plus 2 2 -c 2");
            Assert.AreEqual("6\r\n", context.Output.ToString());
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
    }
}
