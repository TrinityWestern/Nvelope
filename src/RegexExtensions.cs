using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static string Print(this Group group)
        {
            return group.Value;
        }

        public static string Print(this Match match)
        {
            return match.Groups.ToList().Print();
        }
    }
}
