using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    //[CreateAssetMenu]
    public sealed class KRGReferences : ScriptableObject {
        
#region PROPERTIES

        public CharacterDebugText characterDebugTextPrefab { get; private set; }

        public RasterAnimationInfo rasterAnimationInfoPrefab { get; private set; }

#endregion

#region METHODS: ScriptableObject

        void Awake() {
#if NS_TMPRO_PAID
            //these are only for the old PAID version of TextMesh Pro
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMProPaid");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMProPaid");
#elif NS_TMPRO
			//these are only for the new FREE version of TextMesh Pro
			characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMProFree");
			rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMProFree");
#else
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugText");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfo");
#endif
        }

#endregion

    }
}
