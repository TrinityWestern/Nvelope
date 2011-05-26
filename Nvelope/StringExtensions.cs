using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml;

namespace Nvelope
{
    public static class StringExtensions
    {
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
        /// Returns an array of two strings that are separated by the specified string.
        /// </summary>
        /// <param name="source">The source string to be split into two items.</param>
        /// <param name="splitby">The string that the source is to be split by.</param>
        /// <returns>Two strings</returns>
        public static string[] SplitPair(this string source, string splitby)
        {
            string[] res = source.Split(splitby);

            if (res.Count() != 2)
                throw new Exception("The pair was not specified correctly. A pair can only be two items and must be separated by a specified string. In this case, the specified string was '" + splitby + "'.");

            return res;
        }

        /// <summary>
        /// Tries to parse the string as an integer, returning 0 if there's a problem
        /// </summary>
        public static int ToIntOr0(this string source)
        {
            int numericValue;
            return source != null && int.TryParse(source, out numericValue) ? (int)numericValue : 0;
        }

        /// <summary>
        /// Tries to parse the string as a float, returning 0 if there's a problem
        /// </summary>
        public static string ToDoubleAsStringOr0(this string source)
        {
            double numericValue;
            return source != null && double.TryParse(source, out numericValue) ? numericValue.ToString() : "0";
        }

        /// <summary>
        /// Grabs the number in parenthesis (great for autocomplete lookups), returning null if it's not formatted correctly
        /// </summary>
        public static int? GetIntegerInParenthesis(this string source)
        {
            if (source != null)
            {
                Regex numberFinder = new Regex(@"(?:\()(?<number>\d+)(?:\))");
                Match m = numberFinder.Match(source);
                if (m.Success && m.Groups["number"].Success)
                {
                    return m.Groups["number"].Captures[0].Value.ConvertTo<int?>();
                }
                else if (Regex.IsMatch(source, @"^\d+$"))
                {
                    return source.ConvertTo<int?>();
                }
            }
            return null;
        }

        /// <summary>
        /// Grabs the email in parenthesis (great for autocomplete lookups), returning null if it's not formatted correctly
        /// </summary>
        public static string GetEmailInParenthesis(this string source)
        {
            if (source != null)
            {
                Regex emailFinder = new Regex(@"(?:\()(?<email>([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?))(?:\))");
                Match m = emailFinder.Match(source);
                if (m.Success && m.Groups["email"].Success)
                {
                    return m.Groups["email"].Captures[0].Value;
                }
                else if (Regex.IsMatch(source, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    return source;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes all instances of the supplied victims string(s)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="victims"></param>
        /// <returns></returns>
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
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string RemoveEnd(this string source, int count)
        {
            if (source.Length < count) return "";

            return source.Substring(0, source.Length - count);
        }
        
        public static Dictionary<string, string> ToDictionary(this string source, string keyValueSeparator, string itemSeparator)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            NameValueCollection collection = source.ToNameValueCollection(keyValueSeparator, itemSeparator);

            foreach (string key in collection.Keys)
                res.Add(key, collection[key]);

            return res;
        }
        
        /// <summary>
        /// The number of continugous characters at the end of the string that are the charToCheck 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="charToCheck"></param>
        /// <returns></returns>
        public static int NumTimesCharacterRepeatsAtEnd(this string original, char charToCheck)
        {
            var list = new List<char>();
            foreach (var ch in original)
                list.Add(ch);

            list.Reverse();

            int index = 0;
            while (index < list.Count)
            {
                if (list[index] == charToCheck)
                    index++;
                else
                    break;
            }

            return index;            
        }

        /// <summary>
        /// Add a string a specified number of times to the beginning of a string
        /// </summary>
        /// <param name="original">String to add to the beginning of</param>
        /// <param name="stringToAdd">String to add</param>
        /// <param name="numberOfTimes">Number of times to add the string</param>
        /// <returns>Original string with the new one prefixed</returns>
        public static string PrependMultipleTimes(this string original, string stringToAdd, int numberOfTimes)
        {
            string prefix = "";
            for (int i = 1; i <= numberOfTimes; i++)
                prefix += stringToAdd;

            return prefix + original;
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
        /// Tries to pull out an integer from the input string.
        /// </summary>
        /// <param name="input">The value that may or may not have a number in it.</param>
        /// <returns>The number found in the string or zero by default</returns>
        public static string FindIntInString(string input)
        {
            input = input.Trim();
            if (input == "") 
                return "0";
            
            Regex regexPattern = new Regex( @"([-+]?\d+\.?\d*|\.\d+$)");
            MatchCollection matchList = regexPattern.Matches(input);
            if (matchList.Count > 0)
            {
                return matchList[0].ToString();
            }
            else
            {
                return "0";
            }
            
        }
        /// <summary>
        /// Finds any number in a string and return it.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int RetrieveIntInString(string input)
        {
            return FindIntInString(input).ConvertTo<int>();
        }

         /// <summary>
        /// Filters commas and angle brackets from a string and doubles quotes
        /// </summary>
        /// <param name="input">Sting to filter</param>
        /// <returns>Filtered string</returns>
        [Obsolete("What is this used for? If you're trying to clean for XML, use XMLExtensions.EscapeForXml instead")]
        public static string SemiClean(string input)
        {
            if (input == null) return "";
            input = input.Replace("'", "''");
            input = input.Replace("\"", "\"\"");
            input = input.Replace("<", "");
            input = input.Replace(">", "");
            input = input.Replace(",", "");
            return input;
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
        /// <param name="source"></param>
        /// <param name="stringToRemove"></param>
        /// <returns></returns>
        public static string ChopStart(this string source, string stringToRemove)
        {
            source = source.Trim();
            stringToRemove = stringToRemove.Trim();
            if (source.StartsWith(stringToRemove))
                return source.Substring(stringToRemove.Length).Trim();
            else
                return source;
        }

        /// <summary>
        /// Remove whitespace from source, and trim any stringsToRemove if source starts with it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stringsToRemove"></param>
        /// <returns></returns>
        public static string ChopStart(this string source, params string[] stringsToRemove)
        {
            return ChopStart(source, stringsToRemove as IEnumerable<string>);
        }

        /// <summary>
        /// Remove whitespace from source, and trim off any stringsToRemove if source starts with it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stringsToRemove"></param>
        /// <returns></returns>
        public static string ChopStart(this string source, IEnumerable<string> stringsToRemove)
        {
            foreach(var str in stringsToRemove)
                source = source.ChopStart(str);
            return source;
        }

        /// <summary>
        /// Remove whitespace from source, and trim off any stringsToRemove if source ends with it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stringToRemove"></param>
        /// <returns></returns>
        public static string ChopEnd(this string source, string stringToRemove)
        {
            source = source.Trim();
            stringToRemove = stringToRemove.Trim();
            if (source.EndsWith(stringToRemove))
                return source.Substring(0, source.Length - stringToRemove.Length).Trim();
            else
                return source;
        }

        /// <summary>
        /// Remove whitespace from source, and trim off any stringsToRemove if source ends with it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stringsToRemove"></param>
        /// <returns></returns>
        public static string ChopEnd(this string source, params string[] stringsToRemove)
        {
            return ChopEnd(source, stringsToRemove as IEnumerable<string>);
        }

        /// <summary>
        /// Remove whitespace from source, and trim off any stringsToRemove if source ends with it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stringsToRemove"></param>
        /// <returns></returns>
        public static string ChopEnd(this string source, IEnumerable<string> stringsToRemove)
        {
            // Reverse this because we want to be able to remove multiple things if they've 
            // specified it in order,
            // ie, "abcdef".ChopEnd("d","e","f") should return "abc"; but if we don't
            // reverse, we get "abcde"
            foreach (var str in stringsToRemove.Reverse())
                source = source.ChopEnd(str);
            return source;
        }

        /// <summary>
        /// Removes newline from the end of the string
        /// </summary>
        /// <remarks>Stolen from Perl</remarks>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Chomp(this string source)
        {
            return source.ChopEnd(Environment.NewLine);
        }

        /// <summary>
        /// Joins a list of strings together into a single string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> source, string seperator)
        {
            return source.ToSeperatedList(seperator);
        }

        /// <summary>
        /// Repeat a string times times
        /// </summary>
        /// <param name="source"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this string source, int times)
        {
            return new string[] { source }.Repeat(times).Join("");
        }

        /// <summary>
        /// Repeat a char times times
        /// </summary>
        /// <param name="source"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string Repeat(this char source, int times)
        {
            return new string(new char[] { source }.Repeat(times).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string source, params string[] strs)
        {
            return ContainsAll(source, strs as IEnumerable<string>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string source, IEnumerable<string> strs)
        {
            foreach (var str in strs)
                if (!source.Contains(str))
                    return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string source, params string[] strs)
        {
            return ContainsAny(source, strs as IEnumerable<string>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string source, IEnumerable<string> strs)
        {
            foreach (var str in strs)
                if (source.Contains(str))
                    return true;

            return false;
        }

        /// <summary>
        /// Make the string the specified length - truncating or appending spaces as necessary
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToLength(this string source, int length)
        {
            if (source == null)
                return ' '.Repeat(length);

            if (source.Length >= length)
                return source.Substring(0, length);
            else
                return source.Chars().And(' '.Repeat(length - source.Length)).Read();

        }
    }
}
