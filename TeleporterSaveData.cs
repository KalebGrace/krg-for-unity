using UnityEngine;

namespace _0G.Legacy
{
    public struct TeleporterSaveData
    {
        public int gameplaySceneId;
        public int checkpointId;
        public bool activated;

        public TeleporterSaveData(int gameplaySceneId, int checkpointId, bool activated)
        {
            this.gameplaySceneId = gameplaySceneId;
            this.checkpointId = checkpointId;
            this.activated = activated;
        }
    }
}
