using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nvelope
{
    public static class RegexExtensions
    {
        public static IEnumerable<Group> ToList(this GroupCollection coll)
        {
            foreach (var g in coll)
                yield return g as Group;
        }
    }
}
