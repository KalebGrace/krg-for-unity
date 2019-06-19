using UnityEngine;

namespace KRG
{
    public static class FloatEM
    {
        /// <summary>
        /// Is approximately equal to...
        /// </summary>
        public static bool Ap(this float me, float f)
        {
            return Mathf.Approximately(me, f);
        }

        public static Rotation Rotation(this float me)
        {
            return new Rotation(me);
        }

        public static Sign Sign(this float me)
        {
            return new Sign(me);
        }
    }
}
