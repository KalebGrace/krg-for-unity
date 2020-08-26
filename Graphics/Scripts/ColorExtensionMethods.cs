using UnityEngine;

namespace KRG
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