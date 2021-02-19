namespace _0G.Legacy
{
    public class EnumGeneric<T> : EnumGeneric
    {
        public EnumGeneric() : base(typeof(T)) { }

        public EnumGeneric(int intValue) : base(typeof(T), intValue) { }
    }
}