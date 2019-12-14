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

        // TODO: make sure all of these are hooked up!!!

        [Header("Identifiers")]

        [Enum(typeof(AbilityID))]
        public int AbilityID;

        public InputSignature InputSignature;

        [Header("Requirements for Use")]

        public List<Condition> Requirements;

        [Header("Ability Owner Properties")]

        public List<RasterAnimation> OwnerAnimations;

        // TODO: fix this
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


/*
        [Header("Status Effects")]

        [SerializeField]
        [Tooltip("Does this attack cause invulnerability on the damage taker when hit?")]
        protected bool _causesInvulnerability = true;

        [SerializeField]
        [Tooltip("Does this attack cause knock back on the damage taker when hit?")]
        protected bool _causesKnockBack = true;

        //
        //
        [Header("Knock Back (if enabled)")]

        [SerializeField]
        [Tooltip("The instant force impulse added to the target's rigidbody when knocked back. " +
            "The X value should typically be positive. Horizontal flipping will be applied automatically.")]
        protected Vector3 _knockBackForceImpulse = default;

        [SerializeField]
        [Tooltip("How to apply the following Knock Back Distance"
        + " against the corresponding value in the target's Damage Profile.")]
        protected KnockBackCalcMode _knockBackDistanceCalcMode = KnockBackCalcMode.Multiply;

        [SerializeField]
        [Tooltip("Distance (in UNITS) the target is knocked back when damaged.")]
        protected float _knockBackDistance = 1;

        [SerializeField]
        [Tooltip("How to apply the following Knock Back Time"
        + " against the corresponding value in the target's Damage Profile.")]
        protected KnockBackCalcMode _knockBackTimeCalcMode = KnockBackCalcMode.Multiply;

        [SerializeField]
        [Tooltip("Time (in SECONDS) the target is in a knock back state when damaged (overlaps invulnerability time).")]
        protected float _knockBackTime = 1;

        //
        //
        [Header("Attacker Movement")]

        [SerializeField]
        [Tooltip("Distance (in UNITS) the attacker moves horizontally during an attack.")]
        protected float _attackerMoveDistance;

        [SerializeField]
        [Tooltip("Time (in SECONDS) the attacker takes at the start of the attack to move said distance.")]
        protected float _attackerMoveTime;

        [SerializeField]
        [Tooltip("Does attacker movement require directional input?")]
        protected bool _attackerMoveRequiresInput;



        protected virtual void OnValidate()
        {
            _hpDamageRate.floatValue = Mathf.Max(0.0001f, _hpDamageRate.floatValue);
            _maxHitsPerTarget.intValue = Mathf.Max(1, _maxHitsPerTarget.intValue);
            _knockBackTime = Mathf.Max(0, _knockBackTime);
        }



    
        [Header("HP Damage & DPS")]
        
        //seconds between dealing HP Damage (calculated from _hpDamageRate)
        protected float _hpDamageRateSec;

        [SerializeField]
        [Tooltip("Times HP Damage will be dealt per second during a hit."
        + " Setting this to \"false\" makes the attack always deal damage whenever able (e.g. not invulnerable)."
        + " Setting this to \"true\" and specifing an HP Damage Rate can be used to deal very specific DPS:"
        + " E.G. 10 HP Damage * 10 HP Damage Rate = 100 Damage Per Second."
        + " NOTE: This requires setting \"Causes Invulnerability\" to false,"
        + " or dealing damage to someone without it.")]
        [BoolObjectDisable(false, "Whenever Able")]
        protected BoolFloat _hpDamageRate = new BoolFloat(false, 60);

        [SerializeField]
        [Tooltip("Maximum number of \"hits\" (i.e. damage method calls) made by an attack per target."
        + " Setting this to \"false\" makes the attack have no limit"
        + " to the number of damage method calls it can make.")]
        [BoolObjectDisable(false, "No Limit")]
        protected BoolInt _maxHitsPerTarget = new BoolInt(true, 1);


 */
