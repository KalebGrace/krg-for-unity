using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    [CreateAssetMenu(
        fileName = "SomeOne_SomeAbility_Ability.asset",
        menuName = "KRG Scriptable Object/Ability",
        order = 1
    )]
    public class Ability : ScriptableObject
    {
        // SERIALIZED FIELDS

        [Header("Identifiers")]

        [Enum(typeof(AbilityID))]
        public int AbilityID;

        public InputSignature InputSignature;

        [Header("Requirements for Use")]

        public List<Condition> Requirements;

        [Header("Ability Owner Properties")]

        public List<RasterAnimation> OwnerAnimations;

        public List<int> OwnerStatesLocked;

        [Header("Object Definition")]

        public AbilityObject ObjectPrefab;

        [BoolObjectDisable(false, "Unlimited")]
        public BoolInt ObjectLimit = new BoolInt(false, 1);

        [Header("Object Physicality")]

        [Tooltip("The delay (in seconds) between ability use and object activation."
            + " Useful if the owner animation has a wind up.")]
        public float ObjectActivationDelay;

        public bool IsJoinedToOwner;

        public Vector3 Velocity;

        [BoolObjectDisable(false, "Indefinite"), Tooltip("The lifetime (in seconds)"
            + " of the ability's object, which starts upon object activation.")]
        public BoolFloat Lifetime = new BoolFloat(false, 1);

        [Header("Hit Effects")]

        [Tooltip("The prefab to be instantiated upon hitting a target with this ability.")]
        public GameObject HitVFXPrefab;

        public List<Flag> HitFlags;

        [System.Serializable]
        public struct Flag
        {
            [Enum(typeof(AbilityFlag))]
            public int Value;
        }

        public List<Effector> Effectors;

        [Header("Audio Events (e.g. Sound Effects)")]

        [AudioEvent, Tooltip("The audio event to be played upon using this ability.")]
        public string UseSound;

        [AudioEvent, Tooltip("The audio event to be played upon activating this ability's object.")]
        public string ObjectActivationSound;

        [Header("Associated Abilities")]

        public List<AbilityChain> ChainedAbilities;

        // PUBLIC METHODS

        public RasterAnimation GetRandomOwnerAnimation()
        {
            int count = OwnerAnimations?.Count ?? 0;
            if (count == 0) return null;
            int i = Random.Range(0, count);
            return OwnerAnimations[i];
        }

        public virtual bool IsAvailable(AbilityState abil)
        {
            return !abil.IsOutsideOfChainScope && Requirements.AreConditionsMet(abil.AbilityOwner.Body);
        }

        public virtual void ConsumeResources(AbilityState abil)
        {
            Requirements.Consume(abil.AbilityOwner.Body);
        }

        public virtual void PlayUseSound(AbilityState abil)
        {
            G.audio.PlaySFX(UseSound, abil.AbilityOwner.Body.transform.position);
        }
    }
}
