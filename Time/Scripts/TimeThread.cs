using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public class TimeThread : ITimeThread {

        event System.Action _pauseHandlers;
        event System.Action _unpauseHandlers;

        System.Action _callbackPause;
        System.Action _callbackUnpause;
        int? _freezePauseKey;
        float _freezeTime;
        List<int> _pauseKeys = new List<int>();
        bool _isAppThread;
        TimeRate _timeRate = TimeRate.Scaled;
        TimeRate _timeRateQueued = TimeRate.Scaled;
        TimeRate _timeRateUnpause = TimeRate.Scaled;
        float _timeScale = 1;
        List<TimeTrigger> _triggers = new List<TimeTrigger>();
        List<TimeTrigger> _triggersNew = new List<TimeTrigger>();
        List<TimeTrigger> _triggersOut = new List<TimeTrigger>();
        
#if NS_DG_TWEENING
        readonly List<Tween> _tweens = new List<Tween>();
#endif

#region Properties

        public float deltaTime {
            get {
                switch (_timeRate) {
                    case TimeRate.Paused:
                        return 0f;
                    case TimeRate.Scaled:
                        return _timeScale * Time.deltaTime;
                    case TimeRate.Unscaled:
                        return Time.unscaledDeltaTime;
                    default:
                        G.U.Unsupported(this, _timeRate);
                        return 0f;
                }
            }
        }

        public float fixedDeltaTime {
            get {
                switch (_timeRate) {
                    case TimeRate.Paused:
                        return 0f;
                    case TimeRate.Scaled:
                        return _timeScale * Time.fixedDeltaTime;
                    case TimeRate.Unscaled:
                        return Time.fixedUnscaledDeltaTime;
                    default:
                        G.U.Unsupported(this, _timeRate);
                        return 0f;
                }
            }
        }

        public bool isPaused { get { return _timeRate == TimeRate.Paused; } }

        //TODO: is this used publicly in SoAm? there is nothing in the interface for it
        public bool isTimeRateQueued { get { return _timeRate != _timeRateQueued; } }

        /// <summary>
        /// Gets the speed of this specific time thread.
        /// If paused, it will be 0. If unscaled, it will be 1. If scaled, it wll return this thread's timeScale value.
        /// </summary>
        /// <value>The speed.</value>
        public float speed {
            get {
                switch (_timeRate) {
                    case TimeRate.Paused:
                        return 0f;
                    case TimeRate.Scaled:
                        return _timeScale;
                    case TimeRate.Unscaled:
                        return 1f;
                    default:
                        G.U.Unsupported(this, _timeRate);
                        return 0f;
                }
            }
        }

        public TimeRate timeRate { get { return _timeRate; } }

        /// <summary>
        /// Gets the time scale of this specific time thread.
        /// NOTE: This is not the same as UnityEngine.Time.timeScale.
        /// </summary>
        /// <value>The time scale.</value>
        public float timeScale { get { return _timeScale; } }

#endregion

        public TimeThread(int index) {
            if (index == (int)TimeThreadInstance.Application) {
                _isAppThread = true;
                _timeRate = TimeRate.Unscaled;
                _timeRateUnpause = _timeRate;
            }
        }

        public void FixedUpdate() {
            CheckFreeze();
            CheckTimeRateQueued();
            if (isPaused) return;
            UpdateTriggers();
        }

#region Methods: Handler

        public void AddPauseHandler(System.Action handler) {
            _pauseHandlers += handler;
        }

        public void AddUnpauseHandler(System.Action handler) {
            _unpauseHandlers += handler;
        }

        public void RemovePauseHandler(System.Action handler) {
            _pauseHandlers -= handler;
        }

        public void RemoveUnpauseHandler(System.Action handler) {
            _unpauseHandlers -= handler;
        }

#endregion

        void InvokePauseHandlers() {
            ObjectManager.InvokeEventActions(ref _pauseHandlers);
        }

        void InvokeUnpauseHandlers() {
            ObjectManager.InvokeEventActions(ref _unpauseHandlers);
        }

#region Methods: Queue

        public void QueueFreeze(float iv, int pauseKey = -2) {
            if (_freezePauseKey.HasValue) {
                if (_freezePauseKey.Value == pauseKey) {
                    _freezeTime = Mathf.Max(iv, _freezeTime);
                } else {
                    G.U.Error("A time freeze has already been queued with a different pause key.");
                }
            } else {
                _freezeTime = iv;
                _freezePauseKey = pauseKey;
                QueuePause(pauseKey);
            }
        }

        public bool QueuePause(int pauseKey) {
            if (_isAppThread) {
                G.U.Error("The TimeRate for the \"Application\" time thread cannot be changed."); 
                return false;
            }
            if (isTimeRateQueued && _timeRateQueued != TimeRate.Paused) { //TODO: re-evaluate this condition
                G.U.Error("A new TimeRate has already been queued.");
                return false;
            }
            // (comment lines added for visual symmetry)
            //
            bool isPauseProcessed = _pauseKeys.Count > 0;
            //
            if (!_pauseKeys.Contains(pauseKey)) {
                _pauseKeys.Add(pauseKey);
            }
            if (isPauseProcessed) {
                //the following code has already been processed, but this is OK, so return true
                return true;
            }
            _timeRateQueued = TimeRate.Paused;
            //TODO: Make sure this happens next frame.
#if NS_DG_TWEENING
            foreach (Tween t in _tweens) {
                t.Pause();
            }
#endif
            return true;
        }

        public void QueuePause(int pauseKey, System.Action callback) {
            //TODO: what should happen to the callback if the time thread is already paused?
            if (!QueuePause(pauseKey)) return;
            _callbackPause += callback;
        }

        public bool QueueUnpause(int pauseKey) {
            if (_isAppThread) {
                G.U.Error("The TimeRate for the \"Application\" time thread cannot be changed."); 
                return false;
            }
            if (isTimeRateQueued && _timeRateQueued != _timeRateUnpause) { //TODO: re-evaluate this condition
                G.U.Error("A new TimeRate has already been queued.");
                return false;
            }
            if (_pauseKeys.Count == 0) {
                //there is nothing left to unpause, but for functional symmetry, return true
                return false;
            }
            if (_pauseKeys.Contains(pauseKey)) {
                _pauseKeys.Remove(pauseKey);
            }
            if (_pauseKeys.Count > 0) {
                //there are still pause keys locking this from being unpaused, but this is OK, so return true
                return true;
            }
            _timeRateQueued = _timeRateUnpause;
            //TODO: Make sure this happens next frame.
#if NS_DG_TWEENING
            foreach (Tween t in _tweens) {
                t.Play();
            }
#endif
            return true;
        }

        public void QueueUnpause(int pauseKey, System.Action callback) {
            //TODO: what should happen to the callback if the time thread is already unpaused?
            if (!QueueUnpause(pauseKey)) return;
            _callbackUnpause += callback;
        }

        public void QueuePauseToggle(int pauseKey) {
            if (!_pauseKeys.Contains(pauseKey)) {
                QueuePause(pauseKey);
            } else {
                QueueUnpause(pauseKey);
            }
        }

        public void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int pauseKey = -1) {
            if (_isAppThread) {
                G.U.Error("The TimeRate for the \"Application\" time thread cannot be changed.");
                return;
            }
            if (isTimeRateQueued) {
                G.U.Error("A new TimeRate has already been queued.");
                return;
            }
            switch (timeRate) {
                case TimeRate.Paused:
                    QueuePause(pauseKey);
                    break;
                case TimeRate.Scaled:
                    _timeRateUnpause = timeRate;
                    _timeScale = timeScale;
                    QueueUnpause(pauseKey);
                    break;
                case TimeRate.Unscaled:
                    _timeRateUnpause = timeRate;
                    QueueUnpause(pauseKey);
                    break;
                default:
                    G.U.Unsupported(this, _timeRate);
                    break;
            }
        }

#endregion

        void CheckFreeze() {
            if (_freezePauseKey.HasValue) {
                if (_freezeTime.IsZero()) {
                    QueueUnpause(_freezePauseKey.Value);
                    _freezePauseKey = null;
                } else {
                    _freezeTime = Mathf.Max(0, _freezeTime - Time.fixedUnscaledDeltaTime);
                }
            }
        }

        void CheckTimeRateQueued() {
            if (!isTimeRateQueued) return;
            switch (_timeRateQueued) {
                case TimeRate.Paused:
                    Pause();
                    break;
                case TimeRate.Scaled:
                case TimeRate.Unscaled:
                    Unpause();
                    break;
                default:
                    G.U.Unsupported(this, _timeRate);
                    break;
            }
        }

        void Pause() {
            _timeRate = TimeRate.Paused;
            /*
			foreach (Sequence s in _tweens) {
				s.Pause();
			}
			*/
            ObjectManager.InvokeEventActions(ref _callbackPause, true);
            InvokePauseHandlers();
        }

        void Unpause() {
            _timeRate = _timeRateUnpause;
            /*
			foreach (Sequence s in _tweens) {
				s.Play();
			}
			*/
            ObjectManager.InvokeEventActions(ref _callbackUnpause, true);
            InvokeUnpauseHandlers();
        }

#region Methods: Trigger

        /// <summary>
        /// Adds a time trigger to this time thread.
        /// </summary>
        /// <returns>The TimeTrigger that was added.</returns>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        /// <param name="handler">HANDLER to be called at the end of the interval.</param>
        /// <param name="disallowFacade">If set to <c>true</c> disallow use of a time trigger facade.</param>
        public TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false) {
            if (iv > 0) {
                TimeTrigger tt = AddTrigger(iv);
                tt.AddHandler(handler);
                return tt;
            } else if (disallowFacade) {
                G.U.Error("The trigger's interval must be greater than zero.");
                return null;
            } else if (iv.IsZero()) {
                //if there is no measurable interval, call the handler immediately with a time trigger facade
                TimeTriggerFacade ttfc = new TimeTriggerFacade(this);
                handler(ttfc);
                return ttfc;
            } else {
                G.U.Error("The trigger's interval must be greater than or equal to zero.");
                return null;
            }
        }

        //TODO: is this used publicly in SoAm? there is nothing in the interface for it
        //NOTE: this is not used publicly in OSH
        public TimeTrigger AddTrigger(float iv) {
            if (iv > 0) {
                TimeTrigger tt = new TimeTrigger(this, iv);
                _triggersNew.Add(tt);
                return tt;
            } else {
                G.U.Error("The trigger's interval must be greater than zero.");
                return null;
            }
        }

        /// <summary>
        /// Links a time trigger to this time thread.
        /// </summary>
        /// <param name="tt">The TimeTrigger to be linked.</param>
        public void LinkTrigger(TimeTrigger tt) {
            if (tt.totalInterval > 0) {
                _triggersNew.Add(tt);
            } else {
                G.U.Error("The trigger's interval must be greater than zero.");
            }
        }

        /// <summary>
        /// Removes a time trigger from this time thread. It will be disposed and unusable.
        /// </summary>
        /// <returns><c>true</c>, if trigger was removed, <c>false</c> otherwise.</returns>
        /// <param name="tt">The TimeTrigger to be removed.</param>
        public bool RemoveTrigger(TimeTrigger tt) {
            if (_triggers.Contains(tt) && !tt.isDisposed) {
                tt.Dispose();
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Unlinks a time trigger from this time thread. It will still be usable for later.
        /// </summary>
        /// <returns><c>true</c>, if trigger was unlinked, <c>false</c> otherwise.</returns>
        /// <param name="tt">The TimeTrigger to be unlinked.</param>
        public bool UnlinkTrigger(TimeTrigger tt) {
            if (_triggers.Contains(tt) && !_triggersOut.Contains(tt)) {
                _triggersOut.Add(tt);
                return true;
            } else {
                return false;
            }
        }

#endregion

        void UpdateTriggers() {
            IntegrateTriggers(); //Coming from regular Update to UpdateTriggers.
            for (int i = 0; i < _triggers.Count; i++) {
                _triggers[i].Update(deltaTime);
                if (_triggers[i].isDisposed) {
                    _triggers.RemoveAt(i--);
                }
            }
            IntegrateTriggers(); //Coming from UpdateTriggers back to regular Update.
        }

        void IntegrateTriggers() {
            if (_triggersNew.Count > 0) {
                _triggers.AddRange(_triggersNew);
                _triggersNew.Clear();
                _triggers.Sort();
            }
            if (_triggersOut.Count > 0) {
                _triggers.RemoveAll(_triggersOut.Contains);
                _triggersOut.Clear();
                _triggers.Sort();
            }
        }

#if NS_DG_TWEENING

#region Methods: Tween

        public void AddTween(Tween t) {
            _tweens.Add(t.SetUpdate(UpdateType.Fixed));
        }

        public void RemoveTween(Tween t) {
            _tweens.Remove(t);
        }

#endregion

#endif
        
    }
}
