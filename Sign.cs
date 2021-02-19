namespace _0G.Legacy
{
    public struct Sign : IValue<int>
    {
        public int v { get; set; }

        public Sign(int v)
        {
            this.v = v < 0 ? -1 : (v > 0 ? 1 : 0);
        }

        public Sign(float v)
        {
            this.v = v < 0 ? -1 : (v > 0 ? 1 : 0);
        }

        [System.Flags]
        public enum Flags
        {
            None = 0,
            Neg1AsZero = 1,
            Neg1AsPos1 = 2,
            ZeroAsNeg1 = 4,
            ZeroAsPos1 = 8,
            Pos1AsNeg1 = 16,
            Pos1AsZero = 32,
        }

        public int val(Flags f)
        {
            return convert(v, f);
        }

        static int convert(int i, Flags f)
        {
            if (i < 0)
            { //Neg1
                if (f.has(Flags.Neg1AsZero)) return 0;
                if (f.has(Flags.Neg1AsPos1)) return 1;
            }
            else if (i == 0)
            { //Zero
                if (f.has(Flags.ZeroAsNeg1)) return -1;
                if (f.has(Flags.ZeroAsPos1)) return 1;
            }
            else
            { //Pos1
                if (f.has(Flags.Pos1AsNeg1)) return -1;
                if (f.has(Flags.Pos1AsZero)) return 0;
            }
            return i;
        }

        //shortcut properties

        public float f_1_n1 { get { return (float) val(Flags.ZeroAsPos1); } }
    }

    public static class SignFlagsEM
    {
        public static bool has(this Sign.Flags f, Sign.Flags check)
        {
            return (f & check) > 0;
        }
    }
}