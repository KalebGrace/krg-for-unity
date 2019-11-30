using System.Collections.Generic;
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
        public int saveSlotIndex;
        public int gameplaySceneId;
        public int checkpointId; // for loading position
        public Vector3 position; // for logging only
        public Dictionary<int, float> items;
        public int[] acquiredItems; // DEPRECATED
        public AutoMapSaveData[] autoMaps;
        public Dictionary<int, int> switchStates;
        //etc...

        public static SaveFile New(SaveContext sc)
        {
            return new SaveFile
            {
                version = 1,
                saveContext = sc,
                items = new Dictionary<int, float>(),
                switchStates = new Dictionary<int, int>()
            };
        }

        public string Key => GetKeyPrefix(saveContext) + saveSlotIndex;

        public static string GetKeyPrefix(SaveContext saveContext)
        {
            switch (saveContext)
            {
                case SaveContext.ContinueCheckpoint:
                    return "CheckSaveFile";
                case SaveContext.QuickSave:
                    return "QuickSaveFile";
                case SaveContext.HardSave:
                    return "PermaSaveFile";
            }
            G.U.Err("unknown saveContext", saveContext);
            return "UnknownSaveFile";
        }
    }
}
