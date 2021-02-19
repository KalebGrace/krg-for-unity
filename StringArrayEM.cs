using System.Collections.Generic;

namespace _0G.Legacy
{
    public static class StringArrayEM // string[] extension methods
    {
        public static string[] plus(this string[] me, string to_add)
        {
            string[] ret = new string[me.Length + 1];
            me.CopyTo(ret, 0);
            ret[me.Length] = to_add;
            return ret;
        }

        public static string[] plus(this string[] me, string[] to_add)
        {
            string[] ret = new string[me.Length + to_add.Length];
            me.CopyTo(ret, 0);
            to_add.CopyTo(ret, me.Length);
            return ret;
        }

        public static string[] plus(this string[] me, List<string> to_add)
        {
            string[] ret = new string[me.Length + to_add.Count];
            me.CopyTo(ret, 0);
            to_add.CopyTo(ret, me.Length);
            return ret;
        }
    }
}