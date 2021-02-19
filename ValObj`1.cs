namespace _0G.Legacy
{
    public class ValObj<T> : IValue<T>
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

        public ValObj() { }

        public ValObj(T v)
        {
            this.v = v;
        }
    }
}