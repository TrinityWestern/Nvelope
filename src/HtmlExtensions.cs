using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nvelope
{
    [Obsolete("The code in here should be moved into chocolate")]
    public static class HtmlExtensions
    {
        // Note: fragment id is uchar | reserved, see rfc 1738 page 19
        // puncuation = ? , ; . : !
        // if punctuation is at the end, then don't include it
        public static Regex URL_FORMAT = new Regex(
            @"(?<!\w)((?:https?|ftp):" // protocol + :
            + @"/*(?!/)(?:" // get any starting /'s
            + @"[\w$\+\*@=\-/]" // reserved | unreserved
            + "|%[a-fA-F0-9]{2}" // escape
            + @"|[\?\.:\(\),;!\'](?!(?:\s|$))" // punctuation
            + "|(?:(?<=[^/:]{2})#)" // fragment id
            + "){2,}" // at least two characters in the main url part
            + ")");

        // this code has been ripped from remark
        // (https://launchpad.net/remark)
        // There's a significant difference in this code, it's input
        // is HTML text, instead of a document tree, as a result
        // it doesn't handle SGML entities nicely.
        public static string AutoLink(string htmlsource) 
        {
            var eval = new MatchEvaluator(HtmlExtensions.CreateLink);
            return URL_FORMAT.Replace(htmlsource, eval);

        }

        private static string CreateLink(Match m)
        {
            var url = HttpUtility.HtmlEncode(m.Value);
            return string.Format("<a href='{0}'>{0}</a>", url);
        }
    }
}
