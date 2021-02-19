using UnityEngine;

namespace _0G.Legacy
{
    /// <summary>
    /// HPBar: Hit Point Bar
    /// 1.  HPBar will display the health of an IDamagable game object (e.g. a character that can lose life).
    /// 2.  HPBar is used via G.damage.GetHPBar(IDamagable).Display(parameters).
    /// 3.  HPBar consists of a prefab and a script (attached to said prefab's root game object).
    /// 4.  HPBar can be extended, and a custom HPBar prefab can be assigned in the KRGConfig scriptable object.
    /// </summary>
    public class HPBar : MonoBehaviour
    {
        #region FIELDS : SERIALIZED

        [Header("Parameters")]

        [SerializeField, Enum(typeof(TimeThreadInstance))]
        protected int _timeThreadIndex = (int) TimeThreadInstance.UseDefault;

        [SerializeField, BoolObjectDisable(false, "Always"), Tooltip(
            "The default display duration. If checked, display for this many seconds. If unchecked, display always.")]
        protected BoolFloat _displayDuration = new BoolFloat(true, 2);

        [Header("References")]

        [SerializeField]
        protected GameObject _hpBarFill;

        #endregion

        #region FIELDS : PROTECTED

        protected float _displayDurationMin = 0.01f;
        protected TimeTrigger _displayTimeTrigger;
        protected SpriteRenderer _hpBarFillSR;
        protected Transform _hpBarFillTF;
        protected DamageTaker _target;

        #endregion

        #region PROPERTIES

        public virtual DamageTaker target { get { return _target; } }

        protected virtual TimeThread timeThread
        {
            get
            {
                return G.time.GetTimeThread(_timeThreadIndex, TimeThreadInstance.Gameplay);
            }
        }

        #endregion

        #region METHODS : MonoBehaviour

        protected virtual void Awake()
        {
            if (_hpBarFill != null)
            {
                _hpBarFillSR = _hpBarFill.GetComponent<SpriteRenderer>();
                _hpBarFillTF = _hpBarFill.transform;
            }
        }

        protected virtual void OnDestroy()
        {
            KillDisplayTimer();
        }

        protected virtual void OnValidate()
        {
            _displayDuration.floatValue = Mathf.Max(_displayDurationMin, _displayDuration.floatValue);
        }

        protected virtual void Update()
        {
            float value = _target.HPMax > 0 ? _target.HP / _target.HPMax : 0;
            //size
            _hpBarFillTF.localScale = _hpBarFillTF.localScale.SetX(value);
            _hpBarFillTF.localPosition = _hpBarFillTF.localPosition.SetX((value - 1f) / 2f);
            //color
            _hpBarFillSR.color = value >= 0.7f ? Color.green : (value >= 0.4f ? Color.yellow : Color.red);
        }

        #endregion

        #region METHODS : PUBLIC

        /// <summary>
        /// Display the HPBar for the default display duration specified on the game object / prefab.
        /// If always is set to true, default display duration is ignored; instead, always display the HPBar!
        /// </summary>
        /// <param name="always">If set to <c>true</c> always display the HPBar.</param>
        public void Display(bool always = false)
        {
            Display(!always, false, 0);
        }

        /// <summary>
        /// Display the HPBar for the specified duration.
        /// The duration is for this call only; it does NOT set a new default display duration.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public void Display(float duration)
        {
            if (duration < _displayDurationMin)
            {
                G.U.Err("The duration must be at least {0}, but the provided value was {1}. " +
                    "Did you mean to call Display(bool) or Hide()?", _displayDurationMin, duration);
                return;
            }
            Display(false, true, duration);
        }

        /// <summary>
        /// Hide the HPBar.
        /// </summary>
        public void Hide()
        {
            KillDisplayTimer();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Initialize the HPBar.
        /// </summary>
        public void Init(DamageTaker target)
        {
            _target = target;
            Hide();
        }

        #endregion

        #region METHODS : PROTECTED

        protected virtual void OnDisplay() { }

        #endregion

        #region METHODS : PRIVATE

        void Display(bool useDefault, bool useDuration, float duration)
        {
            KillDisplayTimer();
            gameObject.SetActive(true);
            if (useDefault)
            {
                useDuration = _displayDuration.boolValue;
                duration = _displayDuration.floatValue;
            }
            if (useDuration)
            {
                G.U.Assert(duration >= _displayDurationMin);
                _displayTimeTrigger = timeThread.AddTrigger(duration, Hide);
            }
            OnDisplay();
        }

        void Hide(TimeTrigger tt)
        {
            Hide();
        }

        void KillDisplayTimer()
        {
            if (_displayTimeTrigger != null)
            {
                _displayTimeTrigger.Dispose();
                _displayTimeTrigger = null;
            }
        }

        #endregion
    }
}