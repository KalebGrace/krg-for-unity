using UnityEngine;

namespace KRG
{
    [System.Serializable]
    public struct GraphicData
    {
        public Material BaseSharedMaterial;

        public string IdleRasterAnimationName;

        [System.Obsolete]
        public GraphicsData OldGraphicsData;
    }
}
