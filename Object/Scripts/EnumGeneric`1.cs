using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class EnumGeneric<T> : EnumGeneric {

        public EnumGeneric() : base(typeof(T)) {
        }

        public EnumGeneric(int intValue) : base(typeof(T), intValue) {
        }
    }
}
