using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// This is an ideal base class implementation of IStateOwner.
    /// You may either derive from this, or use your own implementation of IStateOwner.
    /// NOTE: Only classes derived from this will be usable within GameObjectBody/GameObjectRefs.
    /// </summary>
    public abstract class StateOwnerBase : MonoBehaviour, IBodyComponent, IStateOwner
    {
        [SerializeField]
        private GameObjectBody m_Body = default;

        public GameObjectBody Body => m_Body;

        public virtual void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        public virtual void Dispose()
        {
            m_Body.Dispose();
        }

        public abstract void AddStateHandler(ulong state, StateHandler handler);
        public abstract void RemoveStateHandler(ulong state, StateHandler handler);

        public abstract bool HasState(ulong state);
    }
}