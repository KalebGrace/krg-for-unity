using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    /// <summary>
    /// AttackAbility: Attack Ability
    /// 1.  AttackAbility is a body of data that defines an "attack". Specifically, it defines
    ///     the ability to generate this attack, rather than simply an instance of said attack
    ///     (the latter of which is handled by the Attack class).
    /// 2.  AttackAbility is a scriptable object and can be instanced from the asset menu (see CreateAssetMenu below).
    /// 3.  AttackAbility is the nexus of the Attack system, and is used in conjunction with the following classes:
    ///     Attack, AttackAbilityUse, Attacker, AttackString, AttackTarget, and KnockBackCalcMode.
    /// 4.  AttackAbility can -- and SHOULD -- be derived on a per-project basis to allow for (future) extension. All
    ///     scriptable objects should be instanced from the derived class. If this is not done from the beginning, any
    ///     future extension work will require the scriptable objects to be re-created or nested, making things messy.
    /// </summary>
    [CreateAssetMenu(
        fileName = "SomeOne_SomeAttack_AttackAbility.asset",
        menuName = "0G Legacy Scriptable Object/Attack Ability",
        order = 120
    )]
    public class AttackAbility : ScriptableObject
    {
        // CONSTANTS

        public const int VERSION = 2;

        public const float DPS_INTERVAL = 0.05f;

        protected const float FLOAT_DEFAULT = 1.5f;
        protected const float FLOAT_MIN = 0.0001f;

        #region FIELDS : SERIALIZED

        [SerializeField, HideInInspector]
        private int _serializedVersion;

        [SerializeField]
        [Tooltip("An optional string key intended to reference this attack ability.")]
        protected string _attackKey;

        [SerializeField]
        [Tooltip("The prefab that will be instantiated upon attacking.")]
        protected Attack _attackPrefab;

        [SerializeField]
        [Tooltip("The attacker's animation(s) during the attack.")]
        protected AnimationData[] _attackerAnimations;
        //obsolete...
        [HideInInspector]
        [SerializeField]
        [System.Obsolete("Use _attackerAnimations instead.")]
        protected AnimationData m_attackerAnimation;

        [SerializeField, Tooltip("This ability is locked until the attacker obtains this key item.")]
        [Enum(typeof(ItemID))]
        protected int m_KeyItem = default;

        [SerializeField]
        [Tooltip("The input signature for the attack.")]
        protected InputSignature _inputSignature;

        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [Tooltip("The applicable time thread index.")]
        protected int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

        [AudioEvent]
        [SerializeField]
        [Tooltip("The sound effect FMOD event string.")]
        protected string _sfxFmodEvent;

        [SerializeField]
        [Tooltip("Does this sound effect follow the attacker's position?")]
        protected bool _sfxFollowsAttacker;

        //
        //
        [Header("Attack Generation")]

        [SerializeField]
        [Tooltip("The limit of simultaneous attacks.")]
        protected int _attackLimit = 1;

        [SerializeField]
        [Tooltip("Maximum new attacks per second.")]
        protected float _attackRate = FLOAT_DEFAULT;

        [SerializeField]
        [Tooltip("Delay before attack instance becomes active. Useful if the attack animation has a wind up.")]
        protected float _attackDelay = 0;

        //
        //
        [Header("Attack Instance Parameters")]

        [SerializeField]
        [Tooltip("The lifetime of the attack in seconds." +
            " Setting this to \"false\" makes the attack live forever (until explicitly destroyed).")]
        [BoolObjectDisable(false, "Infinite Lifetime")]
        protected BoolFloat _attackLifetime = new BoolFloat(true, 0.5f);

        [SerializeField]
        [Tooltip("Is the attack physically joined to the attacker?" +
            " Setting this to \"false\" makes the attack parent to the hierarchy root and operate in world space.")]
        protected bool _isJoinedToAttacker;

        [SerializeField]
        [Tooltip("Distance traveled by attack in units per second (the speed)." +
            " Using a value of \"0\" makes the attack's local position stationary.")]
        protected float _travelSpeed = FLOAT_DEFAULT;

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
        [Tooltip("Hit point damage dealt by attack." +
            " Using a value of \"0\" makes the attack deal no hit point damage.")]
        protected float _hpDamage = 10;

        [SerializeField]
        [Tooltip("HP Damage dealt per second during contact with the target." +
            " NOTE: This requires setting \"Causes Invulnerability\" to false," +
            " or dealing damage to someone without it.")]
        protected float _hpDamagePerSecond = default;

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
        [Tooltip("The instant force impulse added to the target's rigidbody when knocked back. " +
            "The X value should typically be positive. Horizontal flipping will be applied automatically.")]
        protected Vector3 _knockBackForceImpulse = default;

        [SerializeField]
        [Tooltip("How to apply the following Knock Back Distance" +
            " against the corresponding value in the target's Damage Profile.")]
        protected KnockBackCalcMode _knockBackDistanceCalcMode = KnockBackCalcMode.Multiply;

        [SerializeField]
        [Tooltip("Distance (in UNITS) the target is knocked back when damaged.")]
        protected float _knockBackDistance = 1;

        [SerializeField]
        [Tooltip("How to apply the following Knock Back Time" +
            " against the corresponding value in the target's Damage Profile.")]
        protected KnockBackCalcMode _knockBackTimeCalcMode = KnockBackCalcMode.Multiply;

        [SerializeField]
        [Tooltip("Time (in SECONDS) the target is in a knock back state when damaged (overlaps invulnerability time).")]
        protected float _knockBackTime = 1;

        //
        //
        [Header("Attacker Movement")]

        [SerializeField]
        [Tooltip("Time (in SECONDS) the attacker takes at the start of the attack to move said distance.")]
        protected float _attackerMoveTime;

        public float AttackerMoveSpeed;

        public AnimationCurve AttackerMoveSpeedCurve;

        [SerializeField]
        [Tooltip("Does attacker movement require directional input?")]
        protected bool _attackerMoveRequiresInput;

        [SerializeField, HideInInspector, System.Obsolete]
        protected float _attackerMoveDistance;

        //
        //
        [Header("Effectors")]

        [SerializeField]
        protected List<Effector> effectors = new List<Effector>();

        //
        //
        [Header("Enemy AI Parameters")]

        [FormerlySerializedAs("_aiAttackDistance")]
        public float AI_AttackDistancePCStationary = default;
        public float AI_AttackDistancePCApproach = default;
        public float AI_AttackDistancePCFlee = default;

        #endregion

        #region FIELDS : PROTECTED

        //minimum seconds between new attacks (calculated from _attackRate)
        protected float _attackRateSec;

        //is this a DPS clone?
        //-   a DPS clone is a special attack ability that deals DPS (damage per second) during contact with the target
        protected bool _isDPSClone;

        //applicable time thread interface (from _timeThreadIndex)
        protected TimeThread _timeThread;

        #endregion

        #region PROPERTIES

        public virtual float attackDelay { get { return _attackDelay; } }

        public virtual int attackerAnimationCount
        {
            get
            {
                return _attackerAnimations != null ? _attackerAnimations.Length : 0;
            }
        }

        public virtual bool attackerMoveRequiresInput { get { return _attackerMoveRequiresInput; } }

        public virtual float attackerMoveTime { get { return _attackerMoveTime; } }

        public virtual string attackKey { get { return _attackKey; } }

        public virtual float attackLifetime { get { return _attackLifetime.floatValue; } }

        public virtual int attackLimit { get { return _attackLimit; } }

        public virtual Attack attackPrefab { get { return _attackPrefab; } }

        public virtual float attackRate { get { return _attackRate; } }

        public virtual float attackRateSec
        {
            get
            {
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

        public virtual GameObject hitVFXPrefab { get { return _hitVFXPrefab; } }

        public virtual float hpDamage { get { return _hpDamage; } }

        public virtual InputSignature inputSignature { get { return _inputSignature; } }

        public virtual bool isDPSClone => _isDPSClone;

        public virtual bool isJoinedToAttacker { get { return _isJoinedToAttacker; } }

        public virtual bool IsKeyItemRequired => m_KeyItem != 0;

        public virtual float knockBackDistance { get { return _knockBackDistance; } }

        public virtual KnockBackCalcMode knockBackDistanceCalcMode { get { return _knockBackDistanceCalcMode; } }

        public virtual Vector3 KnockBackForceImpulse => _knockBackForceImpulse;

        public virtual float knockBackTime { get { return _knockBackTime; } }

        public virtual KnockBackCalcMode knockBackTimeCalcMode { get { return _knockBackTimeCalcMode; } }

        public virtual bool requiresDPSClone => !_hpDamagePerSecond.Ap(0);

        public virtual string sfxFmodEvent { get { return _sfxFmodEvent; } }

        public virtual bool sfxFollowsAttacker { get { return _sfxFollowsAttacker; } }

        public virtual TimeThread TimeThread
        {
            get
            {
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

        #region METHODS : MonoBehaviour

        protected virtual void Awake() // GAME BUILD only
        {
            UpdateSerializedVersion();
        }

        protected virtual void OnValidate() // UNITY EDITOR only
        {
            UpdateSerializedVersion();
            _attackLimit = Mathf.Max(1, _attackLimit);
            _attackRate = Mathf.Max(FLOAT_MIN, _attackRate);
            _attackLifetime.floatValue = Mathf.Max(FLOAT_MIN, _attackLifetime.floatValue);
            _knockBackTime = Mathf.Max(0, _knockBackTime);
        }

        protected virtual void OnEnable()
        {
            SetAttackRateSec();
        }

        #endregion

        #region METHODS : PUBLIC

        /// <summary>
        /// Gets the attacker animation.
        /// </summary>
        /// <returns>The attacker animation. Can be null.</returns>
        /// <param name="index">Index. (Use attackerAnimationCount to get count.)</param>
        public virtual AnimationData GetAttackerAnimation(int index)
        {
            G.U.Assert(_attackerAnimations != null);
            if (index < 0 || index >= _attackerAnimations.Length)
            {
                G.U.Err("Invalid index {1} specified. " +
                    "Did you forget to add an attacker animation to the {0} attack ability?", this, index);
            }
            return _attackerAnimations[index];
        }

        public virtual RasterAnimation GetRandomAttackerRasterAnimation()
        {
            int i = Random.Range(0, attackerAnimationCount);
            return GetAttackerAnimation(i) as RasterAnimation;
        }

        /// <summary>
        /// Gets the clone of this attack ability used for DPS (damage per second).
        /// </summary>
        /// <returns>The DPS clone.</returns>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        public virtual AttackAbility GetDPSClone(float iv)
        {
            G.U.Assert(iv > 0);
            AttackAbility aa = Instantiate(this);
            aa._isDPSClone = true;
            aa._sfxFmodEvent = null;
            aa._causesInvulnerability = false;
            aa._hpDamage = aa._hpDamagePerSecond * iv;
            return aa;
        }

        #endregion

        #region METHODS : PROTECTED

        protected virtual void SetTimeThread()
        {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }

        #endregion

        private void SetAttackRateSec()
        {
            _attackRateSec = 1f / _attackRate;
        }

        private void UpdateSerializedVersion()
        {
            while (_serializedVersion < VERSION)
            {
                switch (_serializedVersion)
                {
                    case 0:
#pragma warning disable CS0618 // Type or member is obsolete
                        if (m_attackerAnimation != null)
                        {
                            if (_attackerAnimations == null)
                            {
                                _attackerAnimations = new AnimationData[1];
                            }
                            int ol = _attackerAnimations.Length; //original length
                            if (_attackerAnimations[ol - 1] == null)
                            {
                                _attackerAnimations[ol - 1] = m_attackerAnimation;
                            }
                            else
                            {
                                System.Array.Resize(ref _attackerAnimations, ol + 1);
                                _attackerAnimations[ol] = m_attackerAnimation;
                            }
                            m_attackerAnimation = null;
                        }
#pragma warning restore CS0618 // Type or member is obsolete
                        break;
                    case 1:
#pragma warning disable CS0612 // Type or member is obsolete
                        if (!_attackerMoveDistance.Ap(0))
                        {
                            AttackerMoveSpeed = _attackerMoveDistance / _attackerMoveTime;
                            _attackerMoveDistance = 0;
                        }
#pragma warning restore CS0612 // Type or member is obsolete
                        break;
                }
                ++_serializedVersion;
            }
        }
    }
}