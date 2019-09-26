using UnityEngine;

namespace KRG
{
    /// <summary>
    /// This is a typical base class implementation of IStateOwner.
    /// You may either derive from this, or use your own implementation of IStateOwner.
    /// </summary>
    public abstract class StateOwnerBase : MonoBehaviour, IBodyComponent, IStateOwner
    {
        [SerializeField]
        private GameObjectBody m_Body;

        public GameObjectBody Body => m_Body;

        public virtual void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        public virtual void Dispose()
        {
            m_Body.Dispose();
        }
    }
}
