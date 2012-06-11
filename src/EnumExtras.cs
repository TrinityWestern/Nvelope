namespace Nvelope
{
    using System;
    using System.Collections.Generic;

	public static class EnumExtras
    {
        public static List<T> ToList<T>(){
            var t = typeof(T);
            var result = new List<T>();
            foreach (var v in Enum.GetValues(t))
            {
                result.Add((T)v);
            }
            return result;
        }
    }
}
