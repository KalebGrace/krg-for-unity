using UnityEngine;

namespace KRG
{
    public sealed class KRGReferences : ScriptableObject
    {
        public CharacterDebugText characterDebugTextPrefab { get; private set; }

        public RasterAnimationInfo rasterAnimationInfoPrefab { get; private set; }

        void Awake()
        {
            //these are only for the new Unity Package Manager version of TextMesh Pro
            characterDebugTextPrefab = Resources.Load<CharacterDebugText>("Character/CharacterDebugTextTMPro");
            rasterAnimationInfoPrefab = Resources.Load<RasterAnimationInfo>("Graphics/RasterAnimationInfoTMPro");
        }
    }
}
