using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    //[CreateAssetMenu]
    public sealed class KRGReferences : ScriptableObject {

#region serialized fields

        [SerializeField]
        CharacterDebugText _characterDebugTextPrefab;

        [SerializeField]
        RasterAnimationInfo _rasterAnimationInfoPrefab;

#endregion

#region properties

        public CharacterDebugText characterDebugTextPrefab { get { return _characterDebugTextPrefab; } }

        public RasterAnimationInfo rasterAnimationInfoPrefab { get { return _rasterAnimationInfoPrefab; } }

#endregion

    }
}
