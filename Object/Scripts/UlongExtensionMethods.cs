using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public static class UlongExtensionMethods {

        public static bool HasFlag(this ulong flagsEnum, ulong flag) {
            return (flagsEnum & flag) == flag;
        }
    }
}
