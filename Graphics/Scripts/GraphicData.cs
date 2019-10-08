using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    [System.Serializable]
    public struct GraphicData
    {
        public Material BaseSharedMaterial;

        public string IdleAnimationName;

        public List<StateAnimation> StateAnimations;
    }
}
