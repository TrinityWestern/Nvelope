using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nvelope.Reading
{
    public static class TypeConversion
    {
        /// <summary>
        /// Try to guess what type the strVal might represent
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        public static Type GuessType(string strVal)
        {
            if (strVal == "NULL" || strVal == null)
                return null;

            var type = _infervertConversions.First(r => r.Key.IsMatch(strVal)).Value;
            return type;
        }

        /// <summary>
        /// Based on a string representation, try to convert the value to the "appropriate" type
        /// Largely guesswork
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        public static object Infervert(string strVal)
        {
            if (strVal == "NULL")
                return null;

            var type = GuessType(strVal);
            return strVal.ConvertTo(type);
        }

        /// <summary>
        /// Used by Infervert, this is how we guess what type data is supposed to be
        /// Huge assumptions and guesswork here
        /// </summary>
        private static Dictionary<Regex, Type> _infervertConversions = new Dictionary<Regex, Type>()
        {
            {new Regex("^[0-9]+$", RegexOptions.Compiled), typeof(int)},
            {new Regex("^[0-9]+\\.[0-9]+$", RegexOptions.Compiled), typeof(decimal)},
            {new Regex("^[tT]rue|[Ff]alse$", RegexOptions.Compiled), typeof(bool)},
            {new Regex("^[0-9]{4}\\-[0-9]{2}\\-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}\\.[0-9]{3}$", 
                RegexOptions.Compiled), typeof(DateTime)},
            {new Regex(".*", RegexOptions.Compiled), typeof(string)}
        };
    }
}
