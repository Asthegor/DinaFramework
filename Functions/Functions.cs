using System;

namespace DinaFramework.Functions
{
    public static class DinaFunctions
    {
        public static string ExtractValue(ref string value, string sep)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(sep))
                return value;
            string res;
            if (!value.Contains(sep, StringComparison.CurrentCulture))
            {
                res = value;
                value = "";
            }
            else
            {
                int posSep = value.IndexOf(sep, StringComparison.CurrentCulture);
                res = value[..posSep];
                value = value[(posSep + sep.Length)..];
            }
            return res;
        }
    }
}
