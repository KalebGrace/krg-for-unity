using UnityEngine;

namespace KRG
{
    [CreateAssetMenu(
        fileName = "SomeOne_CharacterDossier.asset",
        menuName = "KRG Scriptable Object/Character Dossier",
        order = 308
    )]
    public sealed class CharacterDossier : Docket
    {
        // CONSTANTS

        public const string CHARACTER_DOSSIER_SUFFIX = "_CharacterDossier";
        public const string IDLE_ANIMATION_SUFFIX = "_Idle_RasterAnimation";

        // SERIALIZED FIELDS

        [Header("Game Object Data")]

        public string FullName;

        [Enum(typeof(CharacterID))]
        public int CharacterID;

        public CharacterType CharacterType;

        [Header("Character Data")]

        public CharacterData Data;

        [Header("Graphic Data")]

        public GraphicData GraphicData;

        // PROPERTIES

        public override int ID => CharacterID;
        public override string BundleName => GetBundleName(CharacterID);
        public override string DocketSuffix => CHARACTER_DOSSIER_SUFFIX;
        public override string DefaultAnimationSuffix => IDLE_ANIMATION_SUFFIX;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrWhiteSpace(FullName))
            {
                FullName = FileName;
            }

            DefaultIdleAnimationName();

            for (int i = 0; i < GraphicData.StateAnimations.Count; ++i)
            {
                StateAnimation sa = GraphicData.StateAnimations[i];

                string aniName = sa.animationName;

                if (!string.IsNullOrWhiteSpace(aniName) && !aniName.Contains("_"))
                {
                    sa.animationName = string.Format("{0}_{1}_RasterAnimation", FileName, aniName);

                    GraphicData.StateAnimations[i] = sa;
                }
            }
        }

        public void DefaultIdleAnimationName()
        {
            if (string.IsNullOrWhiteSpace(GraphicData.IdleAnimationName))
            {
                GraphicData.IdleAnimationName = FileName + IDLE_ANIMATION_SUFFIX;
            }
        }

        public static string GetBundleName(int characterID)
        {
            return "_c" + characterID.ToString("D5");
        }
    }
}