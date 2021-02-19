namespace _0G.Legacy
{
    public struct Rotation : IValue<float>
    {
        public float v { get; set; }

        public Rotation(float v)
        {
            this.v = v;
        }

        public float clamped_0_360x
        {
            get
            {
                var d = v;
                while (d < 0) d += 360;
                return d %= 360;
            }
        }

        public Direction directionCardinal
        {
            get
            {
                var d = clamped_0_360x;
                return d >= 315 || d < 45 ? Direction.North :
                    d < 135 ? Direction.East :
                    d < 225 ? Direction.South :
                    Direction.West;
            }
        }

        public Direction directionOrdinal
        {
            get
            {
                var d = clamped_0_360x;
                return d >= 337.5f || d < 22.5f ? Direction.North :
                    d < 67.5f ? Direction.NorthEast :
                    d < 112.5f ? Direction.East :
                    d < 157.5f ? Direction.SouthEast :
                    d < 202.5f ? Direction.South :
                    d < 247.5f ? Direction.SouthWest :
                    d < 292.5f ? Direction.West :
                    Direction.NorthWest;
            }
        }
    }
}