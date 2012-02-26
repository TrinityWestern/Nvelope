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
        public void GetParseErrors()
        {
            var interp = new CommandInterpreter(new StringWriter(), new StringWriter());
            var schema = new CommandSchema() { ArgTypes = typeof(int).And(typeof(int)).ToList() };
            interp.AddCommand<int, int, int>("plus", (i, j) => i + j);

            var errors = interp.ParseErrors("plus 2 abc");
            Assert.AreEqual(1, errors.Count());

        }

        [Test]
        public void Eval()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            var schema = new CommandSchema() { ArgTypes = typeof(int).And(typeof(int)).ToList() };
            interp.AddCommand("plus", schema, (o, e, c) => o.WriteLine(c.Args[0].ConvertTo<int>() + c.Args[1].ConvertTo<int>()));
            
            interp.Eval("plus 2 2");
            Assert.AreEqual("4\r\n", io.ToString());
        }

        [Test]
        public void EvalFromSignature()
        {
            var io = new StringWriter();
            var interp = new CommandInterpreter(io, io);
            interp.AddCommand<int,int,int>("plus", (i, j) => i + j);

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
        public void GetSchemaWithFlag()
        {
            var obj = new CommandObj();
            var mi = obj.GetType().GetMethod("PlusFlag");
            var schema = CommandInterpreter.GetSchema(mi);

            Assert.AreEqual(2, schema.ArgTypes.Count());
            Assert.AreEqual(1, schema.FlagTypes.Count());
        }

        [Test]
        public void ParseCommand()
        {
            var interp = new CommandInterpreter(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            interp.AddCommand("plus", obj, "PlusFlag");

            var command = interp.ParseCommand("plus 2 2 --invert");
            Assert.AreEqual("(2,2)", command.Args.Print());
            Assert.AreEqual("(invert)", command.Flags.Print());
        }


        [Test]
        public void GetParameters()
        {
            var interp = new CommandInterpreter(new StringWriter(), new StringWriter());
            var obj = new CommandObj();
            interp.AddCommand("plus", obj, "PlusFlag");
            var pi = obj.GetType().GetMethod("PlusFlag").GetParameters().OrderBy(p => p.Position);
            var schema = interp.Commands.First().Item2;

            var command = interp.ParseCommand("plus 2 2 --invert");
            var paras = interp.GetParameters(schema, pi, command);
            Assert.AreEqual(3, paras.Count());
            Assert.AreEqual(2, paras.First());
            Assert.AreEqual(2, paras.Second());
            Assert.AreEqual(true, paras.Third());

            command = interp.ParseCommand("plus 2 2");
            paras = interp.GetParameters(schema, pi, command);
            Assert.AreEqual(3, paras.Count());
            Assert.AreEqual(false, paras.Third());
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
