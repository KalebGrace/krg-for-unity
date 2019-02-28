using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class V<T>
    {
        bool __init = true;
        T __initVal;
        T __val;

        public T initVal { get { return __initVal; } }

        public T v
        {
            get { return __val; }
            set
            {
                if (__init)
                {
                    __initVal = value;
                    __init = false;
                }
                __val = value;
            }
        }

        public V()
        {
        }

        public V(T v)
        {
            this.v = v;
        }
    }

    /* STRUCT PATTERN

    public struct V<T>
    {
        bool __notInit;
        T __initVal;
        T __val;

        bool init { get { return !__notInit; } set { __notInit = !value; } }

        public T initVal { get { return __initVal; } }

        public T v
        {
            get { return __val; }
            set
            {
                if (init)
                {
                    __initVal = value;
                    init = false;
                }
                __val = value;
            }
        }

        public V(T v)
        {
            this.v = v;
        }
    }
    
    */
}
