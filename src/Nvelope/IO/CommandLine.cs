using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope;
using System.IO;

namespace Nvelope.IO
{
    ///// <summary>
    ///// Represents a command entered at the command line or repl by a user
    ///// </summary>
    //public struct Command
    //{
    //    public List<object> Args;
    //    public Dictionary<string, object> Switches;
    //    public HashSet<string> Flags;
    //}

    public struct CommandArg
    {
        public string Name;
        public Type Type;
        public bool IsOptional;

        public override string ToString()
        {
            return Name.And(Type.Name).Join(" ")
                + (IsOptional ? "*" : "");
        }
    }

    public struct ParseError
    {
        public CommandArg? Argument;
        public string ArgName;
        public object Value;

        public override string ToString()
        {
            return ArgName;
        }
    }

    public class ParseException : Exception
    {
        public ParseException(IEnumerable<ParseError> errors) : base()
        {
            Errors = errors.ToList();
        }

        public IEnumerable<ParseError> Errors { get; protected set; }
    }

    /// <summary>
    /// Provides methods for implementing a command-line or REPL from C#, and related utilities
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Convert a text command into a dict of arguments, based on the supplied expected arguments.
        /// Throws ParseException if there's a problem
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="expectedArgs"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Parse(string commandText, IEnumerable<CommandArg> expectedArgs = null)
        {
            expectedArgs = SanitizeArgs(expectedArgs);

            var lexed = Lex(commandText);
            var lexErrors = LexErrors(lexed).ToList();
            if (lexErrors.Any())
                throw new ParseException(lexErrors);

            var flags = expectedArgs.Where(a => a.IsOptional && a.Type == typeof(bool)).Select(a => a.Name).ToSet();
            var parsed = ParseArgs(lexed, flags);
            var parseErrors = ParseErrors(parsed).ToList();
            if (parseErrors.Any())
                throw new ParseException(parseErrors);

            var assigned = AssignArgs(parsed, expectedArgs);
            var assignErrors = AssignErrors(assigned, expectedArgs);
            if (assignErrors.Any())
                throw new ParseException(assignErrors);

            var converted = ConvertArgs(assigned, expectedArgs);
            var convertErrors = ConvertErrors(converted, expectedArgs);
            if (convertErrors.Any())
                throw new ParseException(convertErrors);

            return converted.ToDictionary();
        }

        public static IEnumerable<CommandArg> SanitizeArgs(IEnumerable<CommandArg> args)
        {
            if (args == null)
                return new CommandArg[] { };

            var nums = 0.Inc().Select(i => i.ToString()).GetEnumerator();
            return args.Select(a => a.Name != null ? a : new CommandArg() { Name = nums.Pop(), IsOptional = a.IsOptional, Type = a.Type })
                .ToList();
        }

        public static IEnumerable<string> Lex(string commandText)
        {
            return commandText.Tokenize("^\\s*(\"[^\"]*\"|[^\\s]+)")
                .Select(s => s.Trim('"'));
        }

        public static IEnumerable<ParseError> LexErrors(
            IEnumerable<string> tokens)
        {
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="flags">The names of fields that don't require a value</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ParseArgs(IEnumerable<string> tokens, IEnumerable<string> flags)
        {
            if (!tokens.Any())
                return null;

            // If first is a switchname, see if the second is value. If so, take that too
            var firstName = IsName(tokens.First());
            // If the first token is a name, that's the name, otherwise the name is ""
            var name = firstName ? ToName(tokens.First()) : null;
            var firstFlag = firstName && flags.Contains(name);
            var secondVal = tokens.AtLeast(2) && firstName && !IsName(tokens.Second());
            var secondBool = secondVal && tokens.Second().CanConvertTo<bool>();
            
            // If first token is a name, and the second token is a value, second is the value
            // If first token is a name, and the second token isn't a value, the value is ""
            // If the first token is a value, that's the value
            // IF the first token is a flag, then the value is "", unless the second token is a bool
            string val = null;
            if (firstFlag)
                if(secondBool)
                    val = tokens.Second();
                else
                    val = null;
            else if (firstName)
                if (secondVal)
                    val = tokens.Second();
                else
                    val = null;
            else
                val = tokens.First();

            var numTaken = name.And(val).Count(s => s != null);
            var remainder = tokens.Skip(numTaken);

            return new KeyValuePair<string, string>(name, val)
                .And(ParseArgs(remainder, flags));
        }

        public static IEnumerable<ParseError> ParseErrors(
            IEnumerable<KeyValuePair<string, string>> parsedArgs)
        {
            yield break;
        }

        public static IEnumerable<KeyValuePair<string,string>> AssignArgs(
            IEnumerable<KeyValuePair<string,string>> parsedArgs, IEnumerable<CommandArg> expectedArgs)
        {
            var requiredArgs = expectedArgs.Where(a => !a.IsOptional);
            // An infinite sequence of arg names to assign to the args that have no names
            var reqArgNames = requiredArgs.Select(a => a.Name);
            var argNames = reqArgNames
                .And(0.Inc().Select(i => i.ToString()).Except(reqArgNames)) // Start at 0, but don't use any names that requiredArgs have
                .GetEnumerator();

            foreach(var kv in parsedArgs)
                if(kv.Key == null)
                    yield return new KeyValuePair<string,string>(argNames.Pop(), kv.Value);
                else
                    yield return kv;
        }

        public static IEnumerable<ParseError> AssignErrors(
            IEnumerable<KeyValuePair<string, string>> parsedArgs, IEnumerable<CommandArg> expectedArgs)
        {
            if (!expectedArgs.Any())
                yield break;

            var required = expectedArgs.Where(a => !a.IsOptional).Select(a => a.Name).ToSet();
            var supplied = parsedArgs.Select(kv => kv.Key).ToSet();
            var missing = required.Except(supplied);
            foreach (var mi in missing)
            {
                var arg = expectedArgs.Single(a => a.Name == mi);
                yield return new ParseError() { Argument = arg, ArgName = mi };
            }

            var allowed = expectedArgs.Select(a => a.Name).ToSet();
            var invalid = supplied.Except(allowed);
            foreach (var i in invalid)
            {
                var val = parsedArgs.Single(kv => kv.Key == i).Value;
                yield return new ParseError() { ArgName = i, Value = val };
            }
        }

        public static IEnumerable<KeyValuePair<string, object>> ConvertArgs(
            IEnumerable<KeyValuePair<string, string>> parsedArgs, IEnumerable<CommandArg> expectedArgs)
        {
            foreach (var kv in parsedArgs)
            {
                var arg = expectedArgs.Where(c => c.Name == kv.Key);
                var type = arg.Select(a => a.Type).FirstOr(typeof(string));
                var isOptional = arg.Select(a => a.IsOptional).FirstOr(true);
                var value = kv.Value == null? null : kv.Value.ConvertAs(type);

                // Special case: if the arg is optional and a bool, and no value is specified, we treat
                // it as a flag - just by being present it assumes the value true
                // If there's no corresponding arg, and the value is null, it's also a flag
                var isFlag = (isOptional && type == typeof(bool) && value == null) || (!arg.Any() && value == null);
                if(isFlag)
                    value = true;

                yield return new KeyValuePair<string, object>(kv.Key, value);
            }
        }

        public static IEnumerable<ParseError> ConvertErrors(
            IEnumerable<KeyValuePair<string, object>> convertedArgs, IEnumerable<CommandArg> expectedArgs)
        {
            if (!expectedArgs.Any())
                yield break;

            // Check the type of each argument
            foreach (var kv in convertedArgs)
            {
                var arg = expectedArgs.Single(a => a.Name == kv.Key);
                if (kv.Value != null && kv.Value.GetType() != arg.Type)
                    yield return new ParseError() { Argument = arg, ArgName = kv.Key, Value = kv.Value };

                // If the argument is required, and it's null, that's an error too
                if (!arg.IsOptional && kv.Value == null)
                    yield return new ParseError() { Argument = arg, ArgName = kv.Key, Value = kv.Value };
            }
        }

        public static bool IsName(string token)
        {
            return token.StartsWith("-");
        }

        public static string ToName(string token)
        {
            return token.TrimStart('-');
        }



        ///// <summary>
        ///// Takes a string of text (ie, from a command line or repl), and turns it into a Command struct.
        ///// Does automatic type conversion and checking, based on the expected format of the command
        ///// </summary>
        ///// <param name="commandText">The raw text from the command line</param>
        ///// <param name="argTypes">The types of the command arguments. If none supplied, all will be string. If there are more args than
        ///// supplied types, the args at the end with no supplied type will be string</param>
        ///// <param name="switches">A dict of the names to expected Types of the allowable switches. Switches are always -key value pairs,
        ///// and the switch name always must start with -</param>
        ///// <param name="flags">A set of the allowable flags. Flags have no type, they are either present or not</param>
        ///// <returns></returns>
        //public static Command Parse(string commandText, IEnumerable<Type> argTypes = null, Dictionary<string,Type> switches = null, IEnumerable<string> flags = null)
        //{
        //    argTypes = argTypes ?? new Type[] { };
        //    switches = switches ?? new Dictionary<string, Type>();
        //    flags = flags ?? new string[]{};

        //    var paramNames = switches.Keys.And(flags).ToSet();

        //    var rawParts = commandText.Tokenize("^\\s*(\"[^\"]*\"|[^\\s]+)");
        //    var parts = rawParts.Select(s => s.Trim('"')) // Anything that was quoted, unquote
        //        // Anything that starts with - or -- and is a switch or flag, strip off the leading - or --
        //        .Select(s => paramNames.Contains(s.TrimStart('-')) ? s.TrimStart('-') : s);

        //    var suppliedFlags = parts.Intersect(flags);
        //    // Don't use Except below - Except returns the Set difference, which implicitly performs
        //    // a distinct on the sequence.
        //    var argsAndSwitches = parts.Where(p => !flags.Contains(p)).ToList();

        //    // Which switches were actually supplied?
        //    var suppliedSwitches = argsAndSwitches.Intersect(switches.Keys);
            
        //    // For each supplied switch, get it's location, as well as the location of its
        //    // value (which is the next element in the list)
        //    var switchLocs = suppliedSwitches.Select(s => argsAndSwitches.IndexOf(s));
        //    var switchValLocs = switchLocs.Select(i => i + 1);

        //    // Get a mapping of the switch names to the switch values, using the indexes
        //    // we just calculated
        //    var switchVals = switchLocs.Zip(switchValLocs,
        //        (sl, vl) => new KeyValuePair<string,string>(argsAndSwitches[sl], argsAndSwitches[vl]))
        //        .ToDictionary();

        //    // Finally, convert all the switch values to the appropriate type
        //    var typedSwitchVals = switchVals.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value.ConvertTo(switches[kv.Key])))
        //        .ToDictionary();

        //    // The arguments are whatever is left after removing the switches and their values
        //    var args = argsAndSwitches.ExceptIndicies(switchLocs.And(switchValLocs));
        //    // Figure out what the types of the args should be - if we don't have enough, pad with string
        //    argTypes = argTypes.Take(args.Count(), typeof(string));
        //    var typedArgs = args.Zip(argTypes, (val, type) => val.ConvertTo(type));

        //    return new Command()
        //    {
        //        Args = typedArgs.ToList(),
        //        Flags = suppliedFlags.ToSet(),
        //        Switches = typedSwitchVals
        //    };
        //}

    }
}
