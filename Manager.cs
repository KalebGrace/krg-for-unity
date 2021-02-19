using UnityEngine;

namespace _0G.Legacy
{
    public abstract class Manager : IAwake
    {
        public abstract float priority { get; }

        public abstract void Awake();

        protected KRGConfig config { get { return G.config; } }

        protected GameObject gameObject { get { return G.instance.gameObject; } }

        protected MonoBehaviour monoBehaviour { get { return G.instance; } }

        protected Transform transform { get { return G.instance.transform; } }
    }
}