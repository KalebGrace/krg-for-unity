namespace KRG
{
    public struct SaveFile
    {
        public int version;
        public int appState;
        public int activeScene; // sceneId
        public int checkpointId; // for position
        public int[] acquiredItems;
        public float hpMax;
        public float spMax;
        public AutoMapSaveData[] maps;
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
