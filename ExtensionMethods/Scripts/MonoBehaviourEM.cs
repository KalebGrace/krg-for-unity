using UnityEngine;

namespace KRG
{
    public static class MonoBehaviourEM
    {
        public static void Dispose(this MonoBehaviour me)
        {
            G.U.End(me);
        }

        public static T Require<T>(this MonoBehaviour me) where T : Component
        {
            return G.U.Require<T>(me);
        }
    }
}
