using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /*
    /// <summary>
    /// Displays an HP/life/health bar above a character or object that can be damaged.
    /// Used in conjunction with the HPBar prefab and the G.damage.DisplayHPBar(...) method.
    /// </summary>
    */

    /// <summary>
    /// HPBar will display the health of an IDamagable game object.
    /// HPBar is used via G.damage.GetHPBar(IDamagable).Display(parameters).
    /// HPBar consists of a prefab and a script (attached to said prefab's root game object).
    /// </summary>
    public class HPBar : MonoBehaviour {

#region serialized fields

        [Header("Parameters")]

        [SerializeField, Enum(typeof(TimeThreadInstance))]
        protected int _timeThreadIndex = (int)TimeThreadInstance.UseDefault;

        [SerializeField, BoolObjectDisable(false, "Always"), Tooltip(
            "The default display duration. If checked, display for this many seconds. If unchecked, display always.")]
        protected BoolFloat _displayDuration = new BoolFloat(true, 2);

        [Header("References")]

        [SerializeField]
        protected GameObject _hpBarFill;

#endregion

#region protected fields

        protected float _displayDurationMin = 0.01f;
        protected TimeTrigger _displayTimeTrigger;
        protected SpriteRenderer _hpBarFillSR;
        protected Transform _hpBarFillTF;
        protected IDamageable _target;

#endregion

#region properties

        public virtual IDamageable target { get { return _target; } }

        protected virtual ITimeThread timeThread {
            get {
                return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
            }
        }

#endregion

#region MonoBehaviour methods

        protected virtual void Awake() {
            if (_hpBarFill != null) {
                _hpBarFillSR = _hpBarFill.GetComponent<SpriteRenderer>();
                _hpBarFillTF = _hpBarFill.transform;
            }
        }

        protected virtual void OnDestroy() {
            KillDisplayTimer();
        }

        protected virtual void OnValidate() {
            _displayDuration.floatValue = Mathf.Max(_displayDurationMin, _displayDuration.floatValue);
        }

        protected virtual void Update() {
            float value = _target.hpMax > 0 ? _target.hp / _target.hpMax : 0;
            //size
            _hpBarFillTF.localScale = _hpBarFillTF.localScale.SetX(value);
            _hpBarFillTF.localPosition = _hpBarFillTF.localPosition.SetX((value - 1f) / 2f);
            //color
            _hpBarFillSR.color = value >= 0.7f ? Color.green : (value >= 0.4f ? Color.yellow : Color.red);
        }

#endregion

#region public methods

        /// <summary>
        /// Display the HPBar for the default display duration specified on the game object / prefab.
        /// If always is set to true, default display duration is ignored; instead, always display the HPBar!
        /// </summary>
        /// <param name="always">If set to <c>true</c> always display the HPBar.</param>
        public void Display(bool always = false) {
            Display(!always, always, 0);
        }

        /// <summary>
        /// Display the HPBar for the specified duration.
        /// The duration is for this call only; it does NOT set a new default display duration.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public void Display(float duration) {
            if (duration < _displayDurationMin) {
                G.Err("The duration must be at least {0}, but the provided value was {1}. " +
                "Did you mean to call Display(bool) or Hide()?", _displayDurationMin, duration);
                return;
            }
            Display(false, false, duration);
        }

        /// <summary>
        /// Hide the HPBar.
        /// </summary>
        public void Hide() {
            KillDisplayTimer();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Initialize the HPBar.
        /// </summary>
        public void Init(IDamageable target) {
            _target = target;
            Hide();
        }

#endregion

#region private methods

        void Display(bool useDefault, bool always, float duration) {
            KillDisplayTimer();
            gameObject.SetActive(true);
            if (useDefault) {
                always = !_displayDuration.boolValue;
                duration = _displayDuration.floatValue;
            }
            if (!always) {
                G.U.Assert(duration >= _displayDurationMin);
                _displayTimeTrigger = timeThread.AddTrigger(duration, Hide);
            }
        }

        void Hide(TimeTrigger tt) {
            Hide();
        }

        void KillDisplayTimer() {
            if (_displayTimeTrigger != null) {
                _displayTimeTrigger.Dispose();
                _displayTimeTrigger = null;
            }
        }

#endregion

    }
}
