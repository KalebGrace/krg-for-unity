using UnityEngine;

namespace KRG
{
    public static class MonoBehaviourEM
    {
        public static T Require<T>(this MonoBehaviour me) where T : Component
        {
            return G.U.Require<T>(me);
        }
    }
}
