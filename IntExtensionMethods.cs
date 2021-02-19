namespace _0G.Legacy
{
    public static class IntExtensionMethods
    {
        public static bool Between(this int val, int fromInclusive, int toExclusive)
        {
            return val >= fromInclusive && val < toExclusive;
        }

        public static int ClampRotationDegrees(this int deg)
        {
            while (deg < 0) deg += 360;
            return deg % 360;
        }

        public static bool HasFlag(this int flagsEnum, int flag)
        {
            return (flagsEnum & flag) == flag;
        }
    }
}