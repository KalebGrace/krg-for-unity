using System.Collections.Generic;
using _0G.Legacy;
using UnityEngine;

namespace KRG // for Easy Save 3 compatibility; actually _0G.Legacy
{
    /// <summary>
    /// A SaveFile can be used for a checkpoint or hard save.
    /// It will not store meta-game or system data.
    /// </summary>
    public struct SaveFile
    {
        public const int LATEST_VERSION = 5;

        public bool isValid;
        public int version;
        public float gameplayDuration;
        public int gameplaySceneId;
        public int checkpointId; // for loading position upon loading this save
        public Vector3 position; // for resetting position during gameplay only
        public AutoMapSaveData[] autoMaps;
        public TeleporterSaveData[] teleporters;
        public Dictionary<int, int> switchStates;
        public List<int> itemInstancesCollected;
        public Dictionary<int, float> items;
        public Dictionary<int, float> stats;
        public List<BuffData> buffs;

        public static SaveFile New()
        {
            return new SaveFile
            {
                isValid = true,
                version = LATEST_VERSION,
                switchStates = new Dictionary<int, int>(),
                itemInstancesCollected = new List<int>(),
                items = new Dictionary<int, float>(),
                stats = new Dictionary<int, float>()
            };
        }

        public void Validate()
        {
            isValid = false;
            while (version < LATEST_VERSION)
            {
                switch (version)
                {
                    case 5:
                        // code to upgrade from version 5 to version 6 goes here
                        break;
                }
                ++version;
            }
            isValid = true;
        }
    }
}