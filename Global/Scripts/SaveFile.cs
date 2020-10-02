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
        public const int LATEST_VERSION = 3;

        public int version;
        public SaveContext saveContext;
        public int saveSlotIndex;
        public int gameplaySceneId;
        public int checkpointId; // for loading position upon loading this save
        public Vector3 position; // for resetting position during gameplay only
        public int[] acquiredItems; // DEPRECATED as of version 2
        public AutoMapSaveData[] autoMaps;
        public TeleporterSaveData[] teleporters;
        public Dictionary<int, int> switchStates;
        public List<int> itemInstancesCollected;
        public Dictionary<int, float> items;
        public Dictionary<int, float> stats;

        public static SaveFile New(SaveContext sc)
        {
            return new SaveFile
            {
                version = LATEST_VERSION,
                saveContext = sc,
                switchStates = new Dictionary<int, int>(),
                itemInstancesCollected = new List<int>(),
                items = new Dictionary<int, float>(),
                stats = new Dictionary<int, float>()
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

        public void Validate()
        {
            while (version < LATEST_VERSION)
            {
                switch (version)
                {
                    case 1:
                        items = new Dictionary<int, float>();
                        stats = new Dictionary<int, float>();
                        if (acquiredItems != null)
                        {
                            for (int i = 0; i < acquiredItems.Length; ++i)
                            {
                                int itemID = acquiredItems[i];
                                items[itemID] = 1;
                            }
                            acquiredItems = null;
                        }
                        break;
                    case 2:
                        itemInstancesCollected = new List<int>();
                        break;
                }
                ++version;
            }
        }
    }
}