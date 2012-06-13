using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Nvelope.Reflection;
using Nvelope.Collections;

namespace Nvelope.IO
{
    public class CommandInterpreter
    {
        public CommandInterpreter(TextWriter output = null, TextWriter error = null, CommandParser parser = null)
        {
            Output = output ?? new StringWriter();
            Error = error ?? new StringWriter();
            Parser = parser ?? new CommandParser();
        }

        public CommandInterpreter(TextWriter output, TextWriter error, CommandParser parser, object commandObject)
            : this(output, error, parser)
        {
            commandObject.GetType().GetMethods().Each(mi => AddCommand(mi, commandObject));
        }

        public TextWriter Output;
        public TextWriter Error;
        public CommandParser Parser;

        public List<Tuple<string, IEnumerable<CommandArg>, Action<TextWriter, TextWriter, Dictionary<string,object>>>> Commands = 
            new List<Tuple<string, IEnumerable<CommandArg>, Action<TextWriter, TextWriter, Dictionary<string,object>>>>();

        public void AddCommand(string name, IEnumerable<CommandArg> args, Action<TextWriter, TextWriter, Dictionary<string,object>> implementation)
        {
            Commands.Add(Tuple.Create(name, args.ToList() as IEnumerable<CommandArg>, implementation));
        }

        public void AddCommand<T1>(string name, Func<T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string,object>>((o,e,c) =>
                o.WriteLine(fn().Print()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1>(string name, Func<TextWriter, T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o,e,c) =>
                fn(o));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1>(string name, Func<TextWriter, TextWriter, T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Func<T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string,object>>((o, e, c) =>
                o.WriteLine(fn(c.First().Value.ConvertTo<T1>()).Print()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Func<TextWriter, T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Func<TextWriter, TextWriter, T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Func<T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string,object>>((o, e, c) =>
                o.WriteLine(fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()).Print()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Func<TextWriter, T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Func<TextWriter, TextWriter, T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                o.WriteLine(fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(), c.Third().Value.ConvertTo<T3>()).Print()));
            var schema = GetArgs(fn.Method);
            AddCommand(name, schema, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Func<TextWriter, T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Func<TextWriter, TextWriter, T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Func<T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                o.WriteLine(fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(), 
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()).Print()));
            var schema = GetArgs(fn.Method);
            AddCommand(name, schema, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Func<TextWriter, T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Func<TextWriter, TextWriter, T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5, T6>(string name, Func<T1, T2, T3, T4, T5, T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                o.WriteLine(fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>()).Print()));
            var schema = GetArgs(fn.Method);
            AddCommand(name, schema, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5, T6>(string name, Func<TextWriter, T1, T2, T3, T4, T5, T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5, T6>(string name, Func<TextWriter, TextWriter, T1, T2, T3, T4, T5, T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1>(string name, Action<T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string,object>>((o,e,c) =>
                fn(c.First().Value.ConvertTo<T1>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1>(string name, Action<TextWriter, T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1>(string name, Action<TextWriter, TextWriter, T1> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Action<T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Action<TextWriter, T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2>(string name, Action<TextWriter, TextWriter, T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Action<T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(), c.Third().Value.ConvertTo<T3>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Action<TextWriter, T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Action<TextWriter, TextWriter, T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1,T2,T3,T4>(string name, Action<T1,T2,T3,T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                    c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Action<TextWriter, T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Action<TextWriter, TextWriter, T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Action<T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                    c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                    c.Fifth().Value.ConvertTo<T5>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Action<TextWriter, T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5>(string name, Action<TextWriter, TextWriter, T1, T2, T3, T4, T5> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, e, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5, T6>(string name, Action<T1, T2, T3, T4, T5, T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                    c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                    c.Fifth().Value.ConvertTo<T5>(), c.ElementAt(5).Value.ConvertTo<T6>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1, T2, T3, T4, T5, T6>(string name, Action<TextWriter, T1, T2, T3, T4, T5, T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o, e, c) =>
                fn(o, c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>(), c.ElementAt(5).Value.ConvertTo<T6>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand<T1,T2,T3,T4,T5,T6>(string name, Action<TextWriter, TextWriter, T1,T2,T3,T4,T5,T6> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Dictionary<string, object>>((o,e,c) =>
                fn(o,e,c.First().Value.ConvertTo<T1>(), c.Second().Value.ConvertTo<T2>(),
                c.Third().Value.ConvertTo<T3>(), c.Fourth().Value.ConvertTo<T4>(),
                c.Fifth().Value.ConvertTo<T5>(), c.ElementAt(5).Value.ConvertTo<T6>()));
            var args = GetArgs(fn.Method);
            AddCommand(name, args, impl);
        }

        public void AddCommand(MethodInfo mi, object obj, string name = null)
        {
            name = name ?? mi.Name;
            var args = GetArgs(mi);
            var implementation = CreateImplementation(obj, mi);
            AddCommand(name, args, implementation);
        }

        public void AddCommand(string name, object obj, string methodName = null)
        {
            // If they didn't supply a method name, assume it's the name
            methodName = methodName ?? name;

            var mi = obj.GetType().GetMethod(methodName);
            AddCommand(mi, obj, name);
        }

        public IEnumerable<CommandArg> GetArgs(MethodInfo mi)
        {
            var allArgs = mi.GetCommandArgs();
            // We don't want to have the parser try to extract values
            // for the Output and Error textreaders - we'll automatically
            // supply the values for those when we invoke the method.
            // So, for the purposes of parsing the command text, we'll ignore those
            var res = allArgs.SkipWhile(a => a.Type == typeof(TextWriter));
            return res.ToList();
        }

        public Action<TextWriter, TextWriter, Dictionary<string,object>> CreateImplementation(object obj, MethodInfo mi)
        {
            var paras = mi.GetParameters().OrderBy(pi => pi.Position).ToList();

            return new Action<TextWriter, TextWriter, Dictionary<string,object>>((output, error, command) =>
                {
                    var paraList = GetParameters(paras, command);

                    var res = mi.Invoke(obj, paraList);

                    // If the method has a first parameter that is a TextWriter,
                    // we don't need to write anything to the Output - since the method took Output as
                    // a parameter, it is presumably able to handle that itself.
                    if (paraList.Any() && !(paraList.First() is TextWriter))
                        output.WriteLine(res.Print());
                });
        }

        public object[] GetParameters(IEnumerable<ParameterInfo> paras, Dictionary<string,object> command)
        {
            return paras.Select(pi => GetParameter(pi,command)).ToArray();
        }

        public object GetParameter(ParameterInfo pi, Dictionary<string, object> command)
        {
            if (pi.Position == 0 && pi.ParameterType == typeof(TextWriter))
                return Output;

            if (pi.Position == 1 && pi.ParameterType == typeof(TextWriter))
                return Error;

            if (command.ContainsKey(pi.Name))
                return command[pi.Name].ConvertTo(pi.ParameterType);

            if (pi.DefaultValue != DBNull.Value && pi.DefaultValue != null)
                return pi.DefaultValue;

            if (pi.ParameterType.IsNullable())
                return null;

            // Handle flags - if there's no value supplied, but the thing is a bool
            // then default to false
            if (pi.ParameterType == typeof(bool))
                return false;

            throw new InvalidOperationException("Don't know how to get a value for the parameter '" + pi.Name + "'");
        }

        public string ParseCommandName(string text)
        {
            // Chop off the first word and use it as a command name
            return text.SubstringBefore(" ");
        }

        public string ParseCommandText(string text)
        {
            // Chop off the first word and use the rest as command text
            return text.SubstringAfter(" ");
        }

        public void Eval(string text)
        {
            Eval(ParseCommandName(text), ParseCommandText(text));
        }

        public void Eval(string commandName, string commandText)
        {
            var tuple = Commands.FirstOr(t => t.Item1 == commandName, null);
            if(tuple == null)
                throw new InvalidOperationException("No command named '" + commandName + "' is known to the interpreter.");

            var args = tuple.Item2;
            var implementation = tuple.Item3;
            var command = Parser.Parse(commandText, args);
            implementation(Output, Error, command);
        }

        public bool HasCommand(string commandText)
        {
            var name = ParseCommandName(commandText);
            return Commands.Any(c => c.Item1 == name);
        }
    }
}
