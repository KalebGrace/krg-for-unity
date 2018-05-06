using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    [System.Flags]
    public enum Sort {

        None = 0,
        Default = 1 << 0,
        Reverse = 1 << 1,
    }
}
