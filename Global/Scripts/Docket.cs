using UnityEngine;

namespace KRG
{
    public abstract class Docket : ScriptableObject
    {
        // SERIALIZED FIELDS

        [Header("Docket Data")]

        [ReadOnly]
        public string FileName;

        // PROPERTIES

        public abstract int ID { get; }
        //public abstract char Initial { get; }
        public abstract string BundleName { get; }
        public abstract string DocketSuffix { get; }
        public abstract string DefaultAnimationSuffix { get; }

        public virtual string AssetPackBundleName => FileName.ToLower();

        // METHODS

        protected virtual void OnValidate()
        {
            FileName = name.Replace(DocketSuffix, "");
        }
    }
}
