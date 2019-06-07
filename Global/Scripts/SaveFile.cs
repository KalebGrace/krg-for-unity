namespace KRG
{
    public struct SaveFile
    {
        public int version;
        public int appState;
        public int activeScene;
        public int checkpointId; // for position
        public int[] acquiredItems;
        public float hpMax;
        public float spMax;
        //serialized version of map data
        //etc...

        public static SaveFile New()
        {
            return new SaveFile
            {
                version = 1
            };
        }
    }
}
