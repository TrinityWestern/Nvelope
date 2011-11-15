using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nvelope
{
    public static class StringExtensions
    {
        /// <remarks>This exists so that the compiler doesn't get confused and call the IEnumerable[char] version of Print
        /// on a string. Then you would get "(a,b)" from "ab".Print(), which is silly</remarks>
        public static string Print(this string s)
        {
            return s.ToStringN();
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are
        ///     delimited by elements of a specified string array. A parameter specifies
        ///     whether to return empty array elements.
        /// </summary>
        /// <param name="source">The source string, usually the object you're calling the Extension method on.</param>
        /// <param name="splitby">The token to split by.</param>
        public static string[] Split(this string source, string splitby)
        {            
            return source.Split(new string[] { splitby }, StringSplitOptions.None);
        }

        /// <summary>
        /// Removes all instances of the supplied victims string(s)
        /// </summary>
        /// <remarks>This is not the same as Trim</remarks>
        public static string Strip(this string source, params string[] victims)
        {
            foreach (string v in victims)
                source = source.Replace(v, string.Empty);
            return source;
        }

        /// <summary>
        /// Removes the last 'count' number of characters.  If the ask to 
        /// remove a greater number of characters than exist in source
        /// returns an empty string.
        /// </summary>
        public static string RemoveEnd(this string source, int count)
        {
            if (source.Length < count) return "";

            return source.Substring(0, source.Length - count);
        }
        
        /// <summary>
        /// Limit the length of a string while keeping words intact, and adding ... to the end
        /// </summary>
        /// <param name="original">String to make an excerpt of</param>
        /// <param name="maxLength">Maximum length of the string (including the ...)</param>
        /// <returns>Excerpted text</returns>
        public static string Excerpt(this string original, int maxLength)
        {
            string excerpt = "";

            bool broke = false;

            string[] words = original.Split(new char[] { ' ' });

            foreach (string word in words)
            {


                if ((excerpt.Length + word.Length + 1) >= maxLength - 3) // subtract 3 because we are going to add ...
                {
                    broke = true;
                    break;
                }
                else
                {
                    excerpt += word + " ";
                }
            }

            if (broke)
                excerpt += "...";

            return excerpt;
        }
        /// <summary>
        /// Functions like string.Format, only with a dictionary of values
        /// 
        /// From: http://stackoverflow.com/questions/1010123/named-string-format-is-it-possible-c
        /// </summary>
        /// <param name="str">String to form</param>
        /// <param name="dict">Dictionary to retrieve values from</param>
        /// <returns></returns>
        public static string FormatFromDictionary(this string str, IDictionary<string, string> dict)
        {
            int i = 0;
            var newstr = new StringBuilder(str);
            var keyToInt = new Dictionary<string, int>();
            foreach (var tuple in dict) {
                newstr = newstr.Replace("{" + tuple.Key + "}", "{" + i.ToString() + "}");
                keyToInt.Add(tuple.Key, i);
                i++;
            }
            return String.Format(newstr.ToString(), dict.OrderBy(x => keyToInt[x.Key])
                .Select(x => x.Value).ToArray());
        }

        /// <summary>
        /// Converts the input to camelCase
        /// </summary>
        public static string Camelize(this string input, bool capitalizeFirst = false)
        {
            StringBuilder res = new StringBuilder();
            bool prevIsSpace = capitalizeFirst;
            foreach (char c in input.ToCharArray())
                if (c == ' ')
                    prevIsSpace = true;
                else
                    if (prevIsSpace)
                    {
                        res.Append(char.ToUpper(c));
                        prevIsSpace = false;
                    }
                    else
                        res.Append(c);

            return res.ToString();
        }
        /// <summary>
        /// Converts the input from camelCase to space seperated
        /// </summary>
        public static string UnCamelize(this string input, bool unCapitalize = false)
        {
            StringBuilder res = new StringBuilder();
            foreach (char c in input.ToCharArray())
                if (char.IsUpper(c))
                    if (unCapitalize)
                    {
                        res.Append(' ');
                        res.Append(char.ToLower(c));
                    }
                    else
                    {
                        res.Append(' ');
                        res.Append(c);
                    }
                else
                    res.Append(c);
            return res.ToString().TrimStart(' ');
        }

        /// <summary>
        /// Gets the part of the string that comes after the given token. If the given token is not
        /// in the string, this returns null.
        /// </summary>
        /// <param name="source">The source string, usually the object you're calling the Extension method on.</param>
        /// <param name="splitter">The token to look for and use to split the string in two pieces</param>
        public static string SubstringAfter(this string source, string splitter)
        {
            int index = source.IndexOf(splitter);
            if (index > -1)
            {
                return source.Substring(index + splitter.Length);
            }
            return null;
        }

        /// <summary>
        /// Gets the part of the string that comes before the given token. If the given token is not
        /// in the string, this returns null.
        /// </summary>
        /// <param name="source">The source string, usually the object you're calling the Extension method on.</param>
        /// <param name="splitter">The token to look for and use to split the string in two pieces</param>
        public static string SubstringBefore(this string source, string splitter)
        {
            int index = source.IndexOf(splitter);
            if (index > -1)
            {
                return source.Substring(0, index);
            }
            return null;
        }


        /// <summary>
        /// Trim(), but handle nulls, leaving them as nulls
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string TrimN(this string source)
        {
            if (source == null)
                return source;
            else
                return source.Trim();
        }

        /// <summary>
        /// Converts to string, but handles null gracefully (converts to "")
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToStringN(this object source)
        {
            if (source == null)
                return string.Empty;
            else
                return source.ToString();
        }

        /// <summary>
        /// Just a wrapper around String.IsNullOrEmpty so you can call it inline
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return String.IsNullOrEmpty(source);
        }

        private static readonly Regex _tokenizeRegex = new Regex("^\\s*([^\\s]+)", RegexOptions.Compiled);
        /// <summary>
        /// Splits the string by spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<string> Tokenize(this string source)
        {
            return Tokenize(source, _tokenizeRegex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="regexExpression">An regex used to tokenize the string - the entire contents of the expression will
        /// be removed with each iteration, and the value of group 1 will be returned as the token value</param>
        /// <example>"/A/B/C/D".Tokenize("^/([^/]*)") should return (A,B,C,D)</example>
        /// <returns></returns>
        public static IEnumerable<string> Tokenize(this string source, string regexExpression)
        {
            Regex regex = new Regex(regexExpression);
            return Tokenize(source, regex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="regex">An regex used to tokenize the string - the entire contents of the expression will
        /// be removed with each iteration, and the value of group 1 will be returned as the token value</param>
        /// <returns></returns>
        private static IEnumerable<string> Tokenize(this string source, Regex regex)
        {
            var match = regex.Match(source);
            while (match.Success)
            {
                yield return match.Groups[1].Value;
                source = regex.Replace(source, string.Empty);
                match = regex.Match(source);
            }
            if (source != string.Empty)
                yield return source;

        }

        /// <summary>
        /// Remove whitespace from source, and trim off stringToRemove if source starts with it
        /// </summary>
        public static string ChopStart(this string source, params string[] stringsToRemove)
        {
            foreach (var s in stringsToRemove) {
                source = source.Trim();
                if (source.StartsWith(s))
                    source = source.Substring(s.Length);
            }
            return source;
        }

        /// <summary>
        /// Remove whitespace from source, and trim off any stringsToRemove if source ends with it
        /// </summary>
        public static string ChopEnd(this string source, params string[] stringsToRemove)
        {
            foreach (var s in stringsToRemove.Reverse()) {
                source = source.Trim();
                if (source.EndsWith(s))
                    source = source.Substring(0, source.Length - s.Length).Trim();
            }
            return source;
        }

        /// <summary>
        /// Joins a list of strings together into a single string
        /// </summary>
        public static string Join(this IEnumerable<string> source, string seperator)
        {
            return source.ToSeperatedList(seperator);
        }

        /// <summary>
        /// Repeat a string times times
        /// </summary>
        public static string Repeat(this string source, int times)
        {
            return new string[] { source }.Repeat(times).Join("");
        }

        /// <summary>
        /// Repeat a char times times
        /// </summary>
        public static string Repeat(this char source, int times)
        {
            return new string(new char[] { source }.Repeat(times).ToArray());
        }

        public static bool ContainsAll(this string source, params string[] strs)
        {
            return ContainsAll(source, strs as IEnumerable<string>);
        }

        public static bool ContainsAll(this string source, IEnumerable<string> strings)
        {
            foreach (var str in strings)
                if (!source.Contains(str))
                    return false;

            return true;
        }

        public static bool ContainsAny(this string source, params string[] strings)
        {
            return ContainsAny(source, strings as IEnumerable<string>);
        }

        public static bool ContainsAny(this string source, IEnumerable<string> strings)
        {
            foreach (var str in strings)
                if (source.Contains(str))
                    return true;

            return false;
        }

        /// <summary>
        /// Make the string the specified length - truncating or appending spaces as necessary
        /// </summary>
        public static string ToLength(this string source, int length)
        {
            if (source == null)
                return ' '.Repeat(length);

            if (source.Length >= length)
                return source.Substring(0, length);
            else
                return source + ' '.Repeat(length - source.Length);

        }
    }
}
