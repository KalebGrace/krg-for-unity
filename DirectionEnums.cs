namespace _0G.Legacy
{
    /// <summary>
    /// This is the preferred enum for use with serialized fields.
    /// The EnumAttribute will let you craft a custom enum set with ease.
    /// </summary>
    public enum FacingDirection
    {
        Uninitialized = 0,

        /* It's typically nice to set the default as zero,
        * but whenever possible, link values to those in the Direction enum.
        * EXAMPLE:
        
        Right = -3,
        Left = -7,

        */
    }

    /// <summary>
    /// All possible directions are contained within the Direction enum.
    /// Types are classified in DirectionType, further below.
    /// </summary>
    public enum Direction
    {
        // POSITIVE VALUES (including zero)
        // absolute directions in degrees, starting North and going clockwise

        // Cardinal
        North = 0,
        East = 90,
        South = 180,
        West = 270,

        // Ordinal
        NorthEast = 45,
        SouthEast = 135,
        SouthWest = 225,
        NorthWest = 315,

        // NEGATIVE VALUES
        // relative directions of varying types

        // Relative
        Up = -1,
        UpRight = -2,
        Right = -3,
        DownRight = -4,
        Down = -5,
        DownLeft = -6,
        Left = -7,
        UpLeft = -8,

        // Altitude
        Above = -9,
        Below = -10,

        // Unknown
        Unknown = -99,
    }

    public enum DirectionType
    {
        Cardinal = 1,
        Ordinal = 2,
        Relative = 3,
        Altitude = 4,
        Unknown = 5,
    }
}