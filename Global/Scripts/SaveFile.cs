using UnityEngine;

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
        public int gameplaySceneId;
        public int checkpointId; //for loading position
        public Vector3 position; //for logging only
        public int[] acquiredItems; //TODO: change to ItemStack[] items LATER; ItemStack: context (e.g. ItemContext.Acquired), keyItemId, count
        public AutoMapSaveData[] maps;
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
