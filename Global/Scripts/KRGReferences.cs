using UnityEngine;

namespace KRG
{
    public sealed class KRGReferences : ScriptableObject
    {
        public CharacterDebugText characterDebugTextPrefab { get; private set; }

        public RasterAnimationInfo rasterAnimationInfoPrefab { get; private set; }

        void Awake()
        {
#if KRG_X_TMPRO
            //these are only for the new Unity Package Manager version of TextMesh Pro
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMPro");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMPro");
#elif NS_TMPRO_PAID
            //these are only for the OLD PAID Asset Store version of TextMesh Pro
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMProPaid");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMProPaid");
#elif NS_TMPRO
			//these are only for the OLD FREE Asset Store version of TextMesh Pro
			characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMProFree");
			rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMProFree");
#else
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugText");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfo");
#endif
        }
    }
}
