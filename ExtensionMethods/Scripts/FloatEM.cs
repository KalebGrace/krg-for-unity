using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public static class FloatEM
    {
        /// <summary>
        /// Is approximately equal to...
        /// </summary>
        public static bool ap(this float v, float f)
        {
            return Mathf.Approximately(v, f);
        }

        public static Rotation rotation(this float v)
        {
            return new Rotation(v);
        }

        public static Sign sign(this float v)
        {
            return new Sign(v);
        }
    }
}
