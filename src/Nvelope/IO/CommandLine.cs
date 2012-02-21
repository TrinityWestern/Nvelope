using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope;
using System.IO;

namespace Nvelope.IO
{
    /// <summary>
    /// Represents a command entered at the command line or repl by a user
    /// </summary>
    public struct Command
    {
        public List<object> Args;
        public Dictionary<string, object> Switches;
        public HashSet<string> Flags;
    }

    /// <summary>
    /// Provides methods for implementing a command-line or REPL from C#, and related utilities
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Takes a string of text (ie, from a command line or repl), and turns it into a Command struct.
        /// Does automatic type conversion and checking, based on the expected format of the command
        /// </summary>
        /// <param name="commandText">The raw text from the command line</param>
        /// <param name="argTypes">The types of the command arguments. If none supplied, all will be string. If there are more args than
        /// supplied types, the args at the end with no supplied type will be string</param>
        /// <param name="switches">A dict of the names to expected Types of the allowable switches. Switches are always -key value pairs,
        /// and the switch name always must start with -</param>
        /// <param name="flags">A set of the allowable flags. Flags have no type, they are either present or not</param>
        /// <returns></returns>
        public static Command Parse(string commandText, IEnumerable<Type> argTypes = null, Dictionary<string,Type> switches = null, IEnumerable<string> flags = null)
        {
            argTypes = argTypes ?? new Type[] { };
            switches = switches ?? new Dictionary<string, Type>();
            flags = flags ?? new string[]{};

            var paramNames = switches.Keys.And(flags).ToSet();

            var rawParts = commandText.Tokenize("^\\s*(\"[^\"]*\"|[^\\s]+)");
            var parts = rawParts.Select(s => s.Trim('"')) // Anything that was quoted, unquote
                // Anything that starts with - or -- and is a switch or flag, strip off the leading - or --
                .Select(s => paramNames.Contains(s.TrimStart('-')) ? s.TrimStart('-') : s);

            var suppliedFlags = parts.Intersect(flags);
            // Don't use Except below - Except returns the Set difference, which implicitly performs
            // a distinct on the sequence.
            var argsAndSwitches = parts.Where(p => !flags.Contains(p)).ToList();

            // Which switches were actually supplied?
            var suppliedSwitches = argsAndSwitches.Intersect(switches.Keys);
            
            // For each supplied switch, get it's location, as well as the location of its
            // value (which is the next element in the list)
            var switchLocs = suppliedSwitches.Select(s => argsAndSwitches.IndexOf(s));
            var switchValLocs = switchLocs.Select(i => i + 1);

            // Get a mapping of the switch names to the switch values, using the indexes
            // we just calculated
            var switchVals = switchLocs.Zip(switchValLocs,
                (sl, vl) => new KeyValuePair<string,string>(argsAndSwitches[sl], argsAndSwitches[vl]))
                .ToDictionary();

            // Finally, convert all the switch values to the appropriate type
            var typedSwitchVals = switchVals.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value.ConvertTo(switches[kv.Key])))
                .ToDictionary();

            // The arguments are whatever is left after removing the switches and their values
            var args = argsAndSwitches.ExceptIndicies(switchLocs.And(switchValLocs));
            // Figure out what the types of the args should be - if we don't have enough, pad with string
            argTypes = argTypes.Take(args.Count(), typeof(string));
            var typedArgs = args.Zip(argTypes, (val, type) => val.ConvertTo(type));

            return new Command()
            {
                Args = typedArgs.ToList(),
                Flags = suppliedFlags.ToSet(),
                Switches = typedSwitchVals
            };
        }

    }
}
