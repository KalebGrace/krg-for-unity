namespace KRG
{
    /// <summary>
    /// A SaveFile can be used for a checkpoint or hard save.
    /// It will not store meta-game or system data.
    /// </summary>
    public struct SaveFile
    {
        public int version;
        public SaveContext saveContext;
        public int appState;
        public int activeScene; //sceneId
        public int checkpointId; //for position //TODO: for sure
        public int[] acquiredItems;
        public float hpMax; //TODO: remove
        public float spMax; //TODO: remove
        public AutoMapSaveData[] maps; //TODO
        //etc...

        public static SaveFile New(SaveContext sc)
        {
            return new SaveFile
            {
                version = 1,
                saveContext = sc,
            };
        }
    }
}
