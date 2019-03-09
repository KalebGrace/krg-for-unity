using UnityEngine;

namespace KRG
{
    public static class MonoBehaviourEM
    {
        public static void dispose(this MonoBehaviour me)
        {
            G.End(me);
        }

        public static T require<T>(this MonoBehaviour me) where T : Component
        {
            return G.U.Require<T>(me);
        }
    }
}
