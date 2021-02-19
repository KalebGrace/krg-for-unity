using UnityEngine;

namespace _0G.Legacy
{
    public static class ColorExtensionMethods
    {
        public static Color SetAlpha(this Color c, float a)
        {
            c.a = a;
            return c;
        }
    }
}