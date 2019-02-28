using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class Sign : V<bool>
    {
        public Sign(bool v) : base(v)
        {
        }

        public Sign(float v)
        {
            this.v = v > 0;
        }

        public float to_1_0()
        {
            return v ? 1 : 0;
        }

        public float to_1_n1()
        {
            return v ? 1 : -1;
        }
    }
}
