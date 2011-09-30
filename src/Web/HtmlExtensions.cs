using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Nvelope.Web
{
    public static class HtmlExtensions
    {
        public static HtmlString HtmlFormat(this string format, params object[] args)
        {
            var newargs = args.Select(a =>
                a == null ? a : HttpUtility.HtmlEncode(a.ToString()));
            // we're not using special formating, so just use InvariantCulture
            return new HtmlString(
                String.Format(CultureInfo.InvariantCulture, format, args: newargs.ToArray()));
        }
    }
}
