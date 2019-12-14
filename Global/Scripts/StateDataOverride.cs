using UnityEngine;

namespace KRG
{
    public sealed class StateDataOverride : MonoBehaviour, IBodyComponent
    {
        [SerializeField]
        private GameObjectBody m_Body = default;

        public StateDataSerializable StateData;

        public GameObjectBody Body => m_Body;

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }
    }
}
