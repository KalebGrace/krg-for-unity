using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [CreateAssetMenu(
        fileName = "NewKRGDamageProfile.asset",
        menuName = "KRG Scriptable Object/Damage Profile",
        order = 123
    )]
    public class DamageProfile : ScriptableObject {

        //values listed below are simply DEFAULTS; check inspector for actual values

        //applicable time thread index
        [Enum(typeof(TimeThreadInstance))]
        [SerializeField]
        [FormerlySerializedAs("m_timeThreadIndex")]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        //sound effect FMOD event string
#if NS_FMOD
        [FMODUnity.EventRef]
#endif
        [SerializeField]
        [FormerlySerializedAs("m_sfxFmodEvent")]
        string _sfxFmodEvent;

        //base HP (hit point) maximum
        [Header("HP")]
        [SerializeField]
        [FormerlySerializedAs("m_hpMax")]
        float _hpMax = 10;

        //distance (in UNITS) the object is knocked back when damaged
        [Header("Knock Back")]
        [SerializeField]
        [FormerlySerializedAs("m_knockBackDistance")]
        float _knockBackDistance;

        //time (in SECONDS) the object is in a knock back state when damaged (overlaps invulnerability time)
        [SerializeField]
        [FormerlySerializedAs("m_knockBackTime")]
        float _knockBackTime;

        //time (in SECONDS) the object is in an invulnerable state when damaged (overlaps knock back time)
        [Header("Invulnerability")]
        [SerializeField]
        [FormerlySerializedAs("m_invulnerabilityTime")]
        float _invulnerabilityTime;

        //does invulnerability cause the object's graphics to flicker?
        [SerializeField]
        [FormerlySerializedAs("m_invulnerabilityFlicker")]
        bool _invulnerabilityFlicker = true;


        //applicable time thread interface, from _timeThreadIndex
        protected ITimeThread _timeThread;


        public virtual float hpMax { get { return _hpMax; } }

        public virtual float knockBackDistance { get { return _knockBackDistance; } }

        public virtual float knockBackTime { get { return _knockBackTime; } }

        public virtual bool invulnerabilityFlicker { get { return _invulnerabilityFlicker; } }

        public virtual float invulnerabilityTime { get { return _invulnerabilityTime; } }

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


        protected virtual void OnEnable() {
        }

        protected virtual void OnValidate() {
            _hpMax = Mathf.Max(1, _hpMax);
            _knockBackTime = Mathf.Max(0, _knockBackTime);
            _invulnerabilityTime = Mathf.Max(0, _invulnerabilityTime);
        }

        protected virtual void SetTimeThread() {
            _timeThread = G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
        }
    }
}
