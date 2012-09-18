using System;
using System.Collections.Generic;
using System.Linq;

namespace Nvelope.Reading
{
    public static class Read
    {
        public static Dictionary<string, string> Dict(string printedDict)
        {
            return Dict<string, string>(printedDict);
        }


        /// <summary>
        /// The opposite of Dictionary.Print()
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="printedDict"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Dict<TKey, TValue>(string printedDict)
        {
            if (printedDict == null)
                return new Dictionary<TKey, TValue>();

            var str = printedDict.ChopStart("(").ChopEnd(")");
            var pairs = str.Tokenize("^\\s*,?\\s*\\[(((?<BR>\\[)|(?<-BR>\\])|[^\\]\\[])+)\\]\\s*");
            var parsedPairs = pairs.Select(s => splitKeyValue(s));

            return parsedPairs.ToDictionary(t => t.Item1.ConvertTo<TKey>(), t => t.Item2.ConvertTo<TValue>());
        }

        private static Tuple<string, string> splitKeyValue(string pair)
        {
            var parts = pair.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() == 1)
                return Tuple.Create(parts.First(), "");
            else
                return Tuple.Create(parts.First(), parts.Rest().Join(","));
        }

        public static List<string> List(string printedList)
        {
            return List<string>(printedList);
        }

        public static List<T> List<T>(string printedList)
        {
            if (printedList == null)
                return new List<T>();

            var parts = printedList.ChopStart("(").ChopEnd(")")
                .Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);

            return parts.Select(s => s.ConvertTo<T>()).ToList();
        }
    }
}
