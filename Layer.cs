namespace _0G.Legacy
{
    public static class Layer
    {
        public const int None = -1;

        //Builtin Layers:
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        //no Builtin Layer 3 currently
        public const int Water = 4;
        public const int UI = 5;
        //no Builtin Layer 6 currently
        //no Builtin Layer 7 currently

        //User Layers:
        //...
        public const int SpriteGfx = 12;
        public const int SpriteGfxFar = 13;
        //...
        public const int PCBoundBox = 20;
    }
}