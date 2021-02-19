using UnityEngine;

namespace _0G.Legacy
{
    public static class FloatEM // float extension methods
    {
        /// <summary>
        /// Is approximately equal to...
        /// </summary>
        /// <param name="me">The first value to be compared.</param>
        /// <param name="v3">The second value to be compared.</param>
        /// <param name="tolerance">If greater than 0, use this value. Else use Mathf.Approximately.</param>
        public static bool Ap(this float me, float f, float tolerance = 0)
        {
            if (tolerance > 0)
            {
                return Mathf.Abs(me - f) <= tolerance;
            }
            else
            {
                return Mathf.Approximately(me, f);
            }
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