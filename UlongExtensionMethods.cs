namespace _0G.Legacy
{
    public static class UlongExtensionMethods
    {
        public static bool HasFlag(this ulong flagsEnum, ulong flag)
        {
            return (flagsEnum & flag) == flag;
        }
    }
}