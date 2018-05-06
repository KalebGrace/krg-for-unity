using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Attack string.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    [System.Serializable]
    public sealed class AttackString {

#region public constants

        public const int attackStringDepthLimit = 10;

#endregion

#region serialized fields

        [SerializeField]
        [FormerlySerializedAs("m_attackAbility")]
        AttackAbility _attackAbility;

        [SerializeField]
        [FormerlySerializedAs("m_doesInterrupt")]
        bool _doesInterrupt;

#endregion

#region properties

        public AttackAbility attackAbility { get { return _attackAbility; } }

        public bool doesInterrupt { get { return _doesInterrupt; } }

#endregion

    }
}
