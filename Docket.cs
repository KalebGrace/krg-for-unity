using UnityEngine;

namespace _0G.Legacy
{
    public abstract class Docket : ScriptableObject
    {
        // SERIALIZED FIELDS

        [Header("Docket Data")]

        [ReadOnly]
        public string FileName;

        public string ProperName;

        // PROPERTIES

        public abstract int ID { get; }
        //public abstract char Initial { get; }
        public abstract string BundleName { get; }
        public abstract string DocketSuffix { get; }
        public abstract string DefaultAnimationSuffix { get; }

        public virtual string AssetPackBundleName => FileName.ToLower();

        // METHODS

        public virtual void OnValidate()
        {
            FileName = name.Replace(DocketSuffix, "");

            if (string.IsNullOrWhiteSpace(ProperName))
            {
                ProperName = FileName;
            }
        }
    }
}