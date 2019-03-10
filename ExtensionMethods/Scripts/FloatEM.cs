using UnityEngine;

namespace KRG
{
    public static class FloatEM
    {
        /// <summary>
        /// Is approximately equal to...
        /// </summary>
        public static bool Ap(this float v, float f)
        {
            return Mathf.Approximately(v, f);
        }

        public static Rotation Rotation(this float v)
        {
            return new Rotation(v);
        }

        public static Sign Sign(this float v)
        {
            return new Sign(v);
        }
    }
}
