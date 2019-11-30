using UnityEngine;
using UnityEngine.Serialization;

namespace KRG
{
    [System.Obsolete]
    [System.Serializable]
    public sealed class AttackString
    {
        [SerializeField]
        [FormerlySerializedAs("m_attackAbility")]
        private AttackAbility _attackAbility = default;

        [SerializeField]
        [FormerlySerializedAs("m_doesInterrupt")]
        private bool _doesInterrupt = default;

        public AttackAbility AttackAbility => _attackAbility;

        public bool DoesInterrupt => _doesInterrupt;
    }
}
