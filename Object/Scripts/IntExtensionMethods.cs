using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class IntExtensionMethods {

        public static int ClampRotationDegrees(this int deg) {
            while (deg < 0) deg += 360;
            return deg % 360;
        }

        public static bool HasFlag(this int flagsEnum, int flag) {
            return (flagsEnum & flag) == flag;
        }
    }
}
