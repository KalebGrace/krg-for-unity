using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    [CreateAssetMenu(
        fileName = "SomeOne_DamageProfile.asset",
        menuName = "0G Legacy Scriptable Object/Damage Profile",
        order = 401
    )]
    public class DamageProfile : ScriptableObject
    {
        // CONSTANTS

        public const string DAMAGE_PROFILE_SUFFIX = "_DamageProfile";

        // SERIALIZED FIELDS

        [Header("Primary Stats")]

        [SerializeField]
        [Order(-100), Tooltip("Maximum Hit Points (base value, before upgrades)")]
        protected int m_HPMax = 100;

        [Header("Time & Sound")]

        [SerializeField, FormerlySerializedAs("m_timeThreadIndex")]
        [Order(-90), Tooltip("Applicable time thread index")]
        [Enum(typeof(TimeThreadInstance))]
        protected int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

        [SerializeField, FormerlySerializedAs("m_sfxFmodEvent")]
        [Order(-90), Tooltip("Sound effect FMOD event string")]
        [AudioEvent]
        private string _sfxFmodEvent = default;

        [Header("Knock Back")]

        [SerializeField]
        [Order(-80), Tooltip("Is this object immune to knock back?")]
        private bool m_IsImmuneToKnockBack = default;

        //distance (in UNITS) the object is knocked back when damaged
        [SerializeField]
        [FormerlySerializedAs("m_knockBackDistance")]
        float _knockBackDistance = default;

        //time (in SECONDS) the object is in a knock back state when damaged (overlaps invulnerability time)
        [SerializeField]
        [FormerlySerializedAs("m_knockBackTime")]
        float _knockBackTime = default;

        [Header("Invulnerability")]

        //time (in SECONDS) the object is in an invulnerable state when damaged (overlaps knock back time)
        [SerializeField]
        [FormerlySerializedAs("m_invulnerabilityTime")]
        float _invulnerabilityTime = default;

        //does invulnerability cause the object's graphics to flicker?
        [SerializeField]
        [FormerlySerializedAs("m_invulnerabilityFlicker")]
        bool _invulnerabilityFlicker = true;

        [Header("Loot")]

        [FormerlySerializedAs("_knockedOutLoot")]
        public LootData KnockedOutLoot = default;

        [Header("Attack Vulnerabilities")]

        [SerializeField, Tooltip("If none are listed explicity, it is vulnerable to all attacks.")]
        List<AttackAbility> _attackVulnerabilities = new List<AttackAbility>();

        // DEPRECATED

        [SerializeField, HideInInspector, FormerlySerializedAs("m_hpMax")]
        private float _hpMax;

        //applicable time thread interface, from _timeThreadIndex
        protected TimeThread _timeThread;

        // PROPERTIES

        public virtual List<AttackAbility> attackVulnerabilities => _attackVulnerabilities;

        public virtual int HPMin => 0;

        public virtual int HPMax => m_HPMax;

        public virtual bool IsImmuneToKnockBack => m_IsImmuneToKnockBack;

        public virtual float knockBackDistance { get { return _knockBackDistance; } }

        public virtual float knockBackTime { get { return _knockBackTime; } }

        public virtual bool invulnerabilityFlicker { get { return _invulnerabilityFlicker; } }

        public virtual float invulnerabilityTime { get { return _invulnerabilityTime; } }

        public virtual string sfxFmodEvent { get { return _sfxFmodEvent; } }

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

        // METHODS

        protected virtual void OnEnable() { }

        protected virtual void OnValidate()
        {
            if (_hpMax > 0)
            {
                m_HPMax = (int) _hpMax;
                _hpMax = 0;
            }
            m_HPMax = Mathf.Max(1, m_HPMax);
            _knockBackTime = Mathf.Max(0, _knockBackTime);
            _invulnerabilityTime = Mathf.Max(0, _invulnerabilityTime);
        }

        protected virtual void SetTimeThread()
        {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }
    }
}