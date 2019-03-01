using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public struct Sign : IValue<bool>
    {
        public bool v { get; set; }

        public Sign(bool v)
        {
            this.v = v;
        }

        public Sign(float v)
        {
            this.v = v > 0;
        }

        public float to_f_1_0 { get { return v ? 1 : 0; } }

        public float to_f_1_n1 { get { return v ? 1 : -1; } }
    }
}
