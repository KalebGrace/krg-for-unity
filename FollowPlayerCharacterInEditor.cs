using UnityEngine;

namespace _0G.Legacy
{
    [ExecuteAlways]
    public class FollowPlayerCharacterInEditor : MonoBehaviour
    {
        private void Awake()
        {
            if (G.U.IsPlayMode(this)) this.Dispose();
        }

        private void Update()
        {
            GameObjectBody pc = G.U.EditModePlayerCharacter;

            if (pc != null)
            {
                transform.position = pc.transform.position;
            }
        }
    }
}