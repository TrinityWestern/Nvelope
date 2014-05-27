using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Nvelope;
using Nvelope.Reflection;

namespace Nvelope.IO
{
    public static class MethodInfoExtensions
    {
        public static IEnumerable<CommandArg> GetCommandArgs(this MethodInfo mi)
        {
            return mi.GetParameters().OrderBy(pi => pi.Position).Select(GetCommandArg).ToList();
        }

        public static CommandArg GetCommandArg(this ParameterInfo pi)
        {
            var res = new CommandArg();
            res.Name = pi.Name;
            res.Type = pi.ParameterType;
            res.IsOptional = pi.IsOptional || pi.ParameterType.IsNullable();
            return res;
        }
    }
}
