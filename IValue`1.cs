namespace _0G.Legacy
{
    public interface IValue<T>
    {
        T v { get; set; }
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