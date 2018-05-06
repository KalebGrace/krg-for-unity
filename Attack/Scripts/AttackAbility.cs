using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Attack ability.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewKRGAttackAbility.asset",
        menuName = "KRG Scriptable Object/Attack Ability",
        order = 123
    )]
    public class AttackAbility : ScriptableObject {

#region protected constants

        protected const float _floatDefault = 1.5f;
        protected const float _floatMin = 0.0001f;

#endregion

#region serialized fields

        //private serialized data version
        [HideInInspector]
        [SerializeField]
        int _serializedVersion;

        [SerializeField]
        [Tooltip("The prefab that will be instantiated upon attacking.")]
        protected Attack _attackPrefab;

        [SerializeField]
        [Tooltip("The attacker's animation(s) during the attack.")]
        protected KRGAnimation[] _attackerAnimations;
        //obsolete...
        [HideInInspector]
        [SerializeField]
        [System.Obsolete("Use _attackerAnimations instead.")]
        protected KRGAnimation m_attackerAnimation;

        [SerializeField]
        [Tooltip("The input signature for the attack.")]
        protected InputSignature _inputSignature;

        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [Tooltip("The applicable time thread index.")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

#if NS_FMOD
        [FMODUnity.EventRef]
#endif
        [SerializeField]
        [Tooltip("The sound effect FMOD event string.")]
        protected string _sfxFmodEvent;

        //
        //
        [Header("Attack Generation")]

        [SerializeField]
        [Tooltip("The limit of simultaneous attacks.")]
        protected int _attackLimit = 1;

        [SerializeField]
        [Tooltip("Maximum new attacks per second.")]
        protected float _attackRate = _floatDefault;

        //
        //
        [Header("Attack Instance Parameters")]

        [SerializeField]
        [Tooltip("The lifetime of the attack in seconds."
        + " Setting this to \"false\" makes the attack live forever (until explicitly destroyed).")]
        [BoolObjectDisable(false, "Infinite Lifetime")]
        protected BoolFloat _attackLifetime = new BoolFloat(true, 0.5f);

        [SerializeField]
        [Tooltip("Is the attack physically joined to the attacker?"
        + " Setting this to \"false\" makes the attack parent to the hierarchy root and operate in world space.")]
        protected bool _isJoinedToAttacker;

        [SerializeField]
        [Tooltip("Distance traveled by attack in units per second (the speed)."
        + " Using a value of \"0\" makes the attack's local position stationary.")]
        protected float _travelSpeed = _floatDefault;

        //
        //
        [Header("Combos & Strings")]

        [SerializeField]
        [Tooltip("Attack strings associated with this ability while an instance of this attack is active.")]
        protected AttackString[] _attackStrings;

        //
        //
        [Header("Hit Visual Effects")]

        [SerializeField]
        [Tooltip("The prefab that will be instantiated upon hitting a target with this attack.")]
        protected GameObject _hitVFXPrefab;

        //
        //
        [Header("HP Damage & DPS")]

        [SerializeField]
        [Tooltip("Hit point damage dealt by attack."
        + " Using a value of \"0\" makes the attack deal no hit point damage.")]
        protected float _hpDamage = 10;

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

        //
        //
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

#endregion

#region protected fields

        //minimum seconds between new attacks (calculated from _attackRate)
        protected float _attackRateSec;

        //seconds between dealing HP Damage (calculated from _hpDamageRate)
        protected float _hpDamageRateSec;

        //applicable time thread interface (from _timeThreadIndex)
        protected ITimeThread _timeThread;

#endregion

#region properties

        public virtual int attackerAnimationCount {
            get {
                return _attackerAnimations != null ? _attackerAnimations.Length : 0;
            }
        }

        public virtual float attackLifetime { get { return _attackLifetime.floatValue; } }

        public virtual int attackLimit { get { return _attackLimit; } }

        public virtual Attack attackPrefab { get { return _attackPrefab; } }

        public virtual float attackRate { get { return _attackRate; } }

        public virtual float attackRateSec {
            get {
#if UNITY_EDITOR
                SetAttackRateSec();
#endif
                return _attackRateSec;
            }
        }

        public virtual AttackString[] attackStrings { get { return _attackStrings; } }

        public virtual bool causesInvulnerability { get { return _causesInvulnerability; } }

        public virtual bool causesKnockBack { get { return _causesKnockBack; } }

        public virtual bool hasAttackLifetime { get { return _attackLifetime.boolValue; } }

        public virtual bool hasHPDamageRate { get { return _hpDamageRate.boolValue; } }

        public virtual bool hasMaxHitsPerTarget { get { return _maxHitsPerTarget.boolValue; } }

        public virtual GameObject hitVFXPrefab { get { return _hitVFXPrefab; } }

        public virtual float hpDamage { get { return _hpDamage; } }

        public virtual float hpDamageRate { get { return _hpDamageRate.floatValue; } }

        public virtual float hpDamageRateSec {
            get {
#if UNITY_EDITOR
                SetHPDamageRateSec();
#endif
                return _hpDamageRateSec;
            }
        }

        public virtual InputSignature inputSignature { get { return _inputSignature; } }

        public virtual bool isJoinedToAttacker { get { return _isJoinedToAttacker; } }

        public virtual float knockBackDistance { get { return _knockBackDistance; } }

        public virtual KnockBackCalcMode knockBackDistanceCalcMode { get { return _knockBackDistanceCalcMode; } }

        public virtual float knockBackTime { get { return _knockBackTime; } }

        public virtual KnockBackCalcMode knockBackTimeCalcMode { get { return _knockBackTimeCalcMode; } }

        public virtual int maxHitsPerTarget { get { return _maxHitsPerTarget.intValue; } }

        public virtual string sfxFmodEvent { get { return _sfxFmodEvent; } }

        public virtual ITimeThread timeThread {
            get {
#if UNITY_EDITOR
                SetTimeThread();
#else
                if (_timeThread == null) SetTimeThread();
#endif
                return _timeThread;
            }
        }

        public virtual int timeThreadIndex { get { return _timeThreadIndex; } }

        public virtual float travelSpeed { get { return _travelSpeed; } }

#endregion

#region MonoBehaviour methods

        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        protected virtual void Awake() {
            UpdateSerializedVersion();
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        protected virtual void OnValidate() {
            UpdateSerializedVersion();
            _attackLimit = Mathf.Max(1, _attackLimit);
            _attackRate = Mathf.Max(_floatMin, _attackRate);
            _attackLifetime.floatValue = Mathf.Max(_floatMin, _attackLifetime.floatValue);
            _hpDamageRate.floatValue = Mathf.Max(_floatMin, _hpDamageRate.floatValue);
            _maxHitsPerTarget.intValue = Mathf.Max(1, _maxHitsPerTarget.intValue);
            _knockBackTime = Mathf.Max(0, _knockBackTime);
        }

        protected virtual void OnEnable() {
            SetAttackRateSec();
            SetHPDamageRateSec();
        }

#endregion

#region public methods

        /// <summary>
        /// Gets the attacker animation.
        /// </summary>
        /// <returns>The attacker animation. Can be null.</returns>
        /// <param name="index">Index. (Use attackerAnimationCount to get count.)</param>
        public virtual KRGAnimation GetAttackerAnimation(int index) {
            G.U.Assert(_attackerAnimations != null);
            G.U.Assert(index < _attackerAnimations.Length);
            G.U.Assert(index >= 0);
            return _attackerAnimations[index];
        }

#endregion

#region protected methods

        protected virtual void SetTimeThread() {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }

#endregion

#region private methods

        void SetAttackRateSec() {
            _attackRateSec = 1f / _attackRate;
        }

        void SetHPDamageRateSec() {
            _hpDamageRateSec = 1f / _hpDamageRate.floatValue;
        }

        void UpdateSerializedVersion() {
            if (_serializedVersion == 0) {
#pragma warning disable 0618
                if (m_attackerAnimation != null) {
                    if (_attackerAnimations == null) {
                        _attackerAnimations = new KRGAnimation[1];
                    }
                    int ol = _attackerAnimations.Length; //original length
                    if (_attackerAnimations[ol - 1] == null) {
                        _attackerAnimations[ol - 1] = m_attackerAnimation;
                    } else {
                        System.Array.Resize<KRGAnimation>(ref _attackerAnimations, ol + 1);
                        _attackerAnimations[ol] = m_attackerAnimation;
                    }
                    m_attackerAnimation = null;
                }
#pragma warning restore 0618
                _serializedVersion = 1;
            }
        }

#endregion

    }
}
