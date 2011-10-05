namespace Nvelope.Web
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;
    using Nvelope;

    public static class HtmlExtensions
    {
        /// <summary>
        /// An Html version of string.Format.
        /// </summary>
        /// <param name="format">The format for the string this includes html</param>
        /// <param name="args"></param>
        /// <exception cref="FormatException">Thrown when the indexes in the format
        /// string are larger than the number of arguments passed.</exception>
        /// <returns></returns>
        public static HtmlString HtmlFormat(this string format, params object[] args)
        {
            const string FormatSyntax = @"{(?<index>\d+)(?<rest>(?:,\-?\d+)?(?:\:\w+)?)}";

            var func = new Func<object[], Match, string>(HtmlFormatter);
            var eval = new MatchEvaluator(func.Curry(args));

            return new HtmlString(Regex.Replace(format, FormatSyntax, eval));
        }

        private static string HtmlFormatter(object[] args, Match match)
        {
            var indextype = typeof(int);
            var index = (int)indextype.DynamicCast(match.Groups["index"].Value);
            var rest = match.Groups["rest"].Value;
            if (args == null)
            {
                throw new ArgumentNullException("The arguement at index " + index + " was null");
            }
            if (index >= args.Length)
            {
                throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
            }
            if (args[index] == null)
            {
                throw new ArgumentNullException("The arguement at index " + index + " was null");
            }
            var formatted = string.Format(CultureInfo.CurrentCulture, "{0" + rest + "}", args[index]);
            return HttpUtility.HtmlEncode(formatted);
        }
    }
}
