namespace _0G.Legacy
{
    [System.Flags]
    public enum Axis
    {
        None = 0,
        //x
        Xneg = 1 << 0,
        Xpos = 1 << 1,
        X = Xneg + Xpos,
        //y
        Yneg = 1 << 2,
        Ypos = 1 << 3,
        Y = Yneg + Ypos,
        //z
        Zneg = 1 << 4,
        Zpos = 1 << 5,
        Z = Zneg + Zpos,
    }
}