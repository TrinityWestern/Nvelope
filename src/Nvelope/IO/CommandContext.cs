using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Nvelope.Reflection;

namespace Nvelope.IO
{
    public struct CommandSchema
    {
        public List<Type> ArgTypes;
        public Dictionary<string, Type> SwitchTypes;
        public HashSet<string> FlagTypes;
    }

    public class CommandContext
    {
        public CommandContext(TextWriter output, TextWriter error)
        {
            Output = output;
            Error = error;
        }

        public CommandContext(TextWriter output, TextWriter error, object commandObject) : this(output, error)
        {
            Commands = DetermineCommands(commandObject).ToList();
        }

        public List<Tuple<string, CommandSchema, Action<TextWriter, TextWriter, Command>>> Commands = 
            new List<Tuple<string,CommandSchema,Action<TextWriter,TextWriter,Command>>>();

        public void AddCommand(string name, CommandSchema schema, Action<TextWriter, TextWriter, Command> implementation)
        {
            Commands.Add(Tuple.Create(name, schema, implementation));
        }

        public void AddCommand<T1, T2>(string name, Func<T1, T2> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Command>((o,e,c) => 
                o.WriteLine(fn(c.Args.First().ConvertTo<T1>()).Print()));
            var schema = new CommandSchema(){ ArgTypes = typeof(T1).List().ToList() };
            AddCommand(name, schema, impl);
        }

        public void AddCommand<T1, T2, T3>(string name, Func<T1, T2, T3> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Command>((o, e, c) =>
                o.WriteLine(fn(c.Args.First().ConvertTo<T1>(), c.Args.Second().ConvertTo<T2>()).Print()));
            var schema = new CommandSchema() { ArgTypes = typeof(T1).And(typeof(T2)).ToList() };
            AddCommand(name, schema, impl);
        }

        public void AddCommand<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4> fn)
        {
            var impl = new Action<TextWriter, TextWriter, Command>((o, e, c) =>
                o.WriteLine(fn(c.Args.First().ConvertTo<T1>(), c.Args.Second().ConvertTo<T2>(), c.Args.Third().ConvertTo<T3>()).Print()));
            var schema = new CommandSchema() { ArgTypes = typeof(T1).And(typeof(T2)).And(typeof(T3)).ToList() };
            AddCommand(name, schema, impl);
        }

        public void AddCommand(string name, object obj, MethodInfo mi)
        {
            var schema = GetSchema(mi);
            var implementation = GetImplementation(schema, obj, mi);
            AddCommand(name, schema, implementation);
        }

        public void AddCommand(string name, object obj, string methodName)
        {
            var mi = obj.GetType().GetMethod(methodName);
            AddCommand(name, obj, mi);
        }

        public void AddCommand(string name, object obj)
        {
            AddCommand(name, obj, name);
        }

        public static CommandSchema GetSchema(MethodInfo mi)
        {
            var schema = new CommandSchema();
            var paras = mi.GetParameters().OrderBy(pi => pi.Position);
            // If the first one or two paras are TextWriters, then it's an async method, and we should 
            // pass in references to the output and error TextWriters. We assume they are not actual 
            // parameters on the command itself.
            var twAtStart = paras.TakeWhile(pi => pi.ParameterType == typeof(TextWriter)).Count();
            var commParas = paras.Skip(twAtStart);
            
            // The argument parameters are any that aren't optional
            var argParas = commParas.Where(pi => !pi.IsOptional);
            schema.ArgTypes = argParas.Select(pi => pi.ParameterType).ToList();

            // Let's look for any optional parameters - these will become flags (if boolean) or 
            // switches (if any other type)
            var optionalParas = commParas.Where(pi => pi.IsOptional);
            var flags = optionalParas.Where(pi => pi.ParameterType == typeof(bool));

            schema.FlagTypes = flags.Select(pi => pi.Name).ToSet();

            var switches = optionalParas.Where(pi => pi.ParameterType != typeof(bool));
            schema.SwitchTypes = switches.ToDictionary(pi => pi.Name, pi => pi.ParameterType);

            return schema;
        }

        public Action<TextWriter, TextWriter, Command> GetImplementation(CommandSchema schema, object obj, MethodInfo mi)
        {
            var paras = mi.GetParameters().OrderBy(pi => pi.Position).ToList();
            
            return new Action<TextWriter, TextWriter, Command>((output, error, command) =>
                {
                    var paraList = GetParameters(schema, paras, command);

                    var res = mi.Invoke(obj, paraList);

                    // If the method has a first parameter that is a TextWriter,
                    // we don't need to write anything to the Output - since the method took Output as
                    // a parameter, it is presumably able to handle that itself.
                    if (paraList.Any() && !(paraList.First() is TextWriter))
                        output.WriteLine(res.Print());
                });
        }

        public object[] GetParameters(CommandSchema schema, IEnumerable<ParameterInfo> paras, Command command)
        {
            var possibleOptions = schema.FlagTypes.And(schema.SwitchTypes.Keys).ToSet();

            var options = command.Flags.ToDictionary(s => s, s => true as object)
                        .Union(command.Switches);

            // Construct the list of parameters we need to invoke the method
            var usedArgs = 0;
            var paraList = paras.Select(pi =>
            {
                if (pi.Position == 0 && pi.ParameterType == typeof(TextWriter))
                    return Output;
                else if (pi.Position == 1 && pi.ParameterType == typeof(TextWriter))
                    return Error;
                else if (!pi.IsOptional)
                    return command.Args[usedArgs++];
                else if (options.ContainsKey(pi.Name))
                    return options[pi.Name];
                else if (possibleOptions.Contains(pi.Name))
                    return pi.DefaultValue;
                else
                    throw new InvalidOperationException("Don't know how to get a value for the parameter '" + pi.Name + "'");
            });

            return paraList.ToArray();
        }


        public static IEnumerable<Tuple<string, CommandSchema, Action<TextWriter, TextWriter, Command>>> DetermineCommands(object obj)
        {
            yield break;
        }


        public bool ThrowExceptionOnErrors = true;
        public TextWriter Output;
        public TextWriter Error;

        public string ParseCommandName(string commandText)
        {
            // Chop off the first word and use it as a command
            var parts = commandText.Split(' ');
            var commandName = parts[0];
            return commandName;
        }

        public Command ParseCommand(string commandText)
        {
            // Chop off the first word and use it as a command
            var parts = commandText.Split(' ');
            var commandName = parts[0];
            var text = parts.Rest().Join(" ");
            var schema = Commands.Single(t => t.Item1 == commandName);
            var command = CommandLine.Parse(text, schema.Item2.ArgTypes, schema.Item2.SwitchTypes, schema.Item2.FlagTypes);
            return command;
        }

        public void Eval(string commandText)
        {
            Eval(ParseCommandName(commandText), ParseCommand(commandText));
        }

        public void Eval(string commandName, Command command)
        {
            var implementation = Commands.Single(t => t.Item1 == commandName).Item3;
            implementation(Output, Error, command);
        }
    }
}
