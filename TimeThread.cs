using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public class TimeThread
    {
        // PRIVATE EVENTS

        event System.Action _pauseHandlers;
        event System.Action _unpauseHandlers;

        // PRIVATE MEMBERS

        System.Action _callbackPause;
        System.Action _callbackUnpause;
        int? _freezePauseKey;
        float _freezeTime;
        List<object> _pauseKeys = new List<object>();
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

        // PUBLIC PROPERTIES

        public float deltaTime
        {
            get
            {
                switch (_timeRate)
                {
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

        public float fixedDeltaTime
        {
            get
            {
                switch (_timeRate)
                {
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

        public bool isPaused => _timeRate == TimeRate.Paused;

        //TODO: is this used publicly in SoAm?
        public bool isTimeRateQueued => _timeRate != _timeRateQueued;

        /// <summary>
        /// Gets the speed of this specific time thread.
        /// If paused, it will be 0. If unscaled, it will be 1. If scaled, it wll return this thread's timeScale value.
        /// </summary>
        /// <value>The speed.</value>
        public float speed
        {
            get
            {
                switch (_timeRate)
                {
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

        public TimeRate timeRate => _timeRate;

        /// <summary>
        /// Gets the time scale of this specific time thread.
        /// NOTE: This is not the same as UnityEngine.Time.timeScale.
        /// </summary>
        /// <value>The time scale.</value>
        public float timeScale => _timeScale;

        // CONSTRUCTOR

        public TimeThread(int index)
        {
            if (index == (int) TimeThreadInstance.Application)
            {
                _isAppThread = true;
                _timeRate = TimeRate.Unscaled;
                _timeRateUnpause = _timeRate;
            }
        }

        // PUBLIC MONOBEHAVIOUR-LIKE METHODS

        public void FixedUpdate()
        {
            CheckFreeze();
            CheckTimeRateQueued();
            if (isPaused) return;
            UpdateTriggers();
        }

        // PUBLIC HANDLER METHODS

        public void AddPauseHandler(System.Action handler)
        {
            _pauseHandlers += handler;
        }

        public void AddUnpauseHandler(System.Action handler)
        {
            _unpauseHandlers += handler;
        }

        public void RemovePauseHandler(System.Action handler)
        {
            _pauseHandlers -= handler;
        }

        public void RemoveUnpauseHandler(System.Action handler)
        {
            _unpauseHandlers -= handler;
        }

        // PRIVATE HANDLER METHODS

        void InvokePauseHandlers()
        {
            ObjectManager.InvokeEventActions(ref _pauseHandlers);
        }

        void InvokeUnpauseHandlers()
        {
            ObjectManager.InvokeEventActions(ref _unpauseHandlers);
        }

        // PUBLIC QUEUE METHODS

        public void QueueFreeze(float iv, int freezePauseKey = -2)
        {
            if (_freezePauseKey.HasValue)
            {
                if (_freezePauseKey.Value == freezePauseKey)
                {
                    _freezeTime = Mathf.Max(iv, _freezeTime);
                }
                else
                {
                    G.U.Err("A time freeze has already been queued with a different pause key.");
                }
            }
            else
            {
                _freezeTime = iv;
                _freezePauseKey = freezePauseKey;
                QueuePause(freezePauseKey);
            }
        }

        public bool QueuePause(object pauseKey)
        {
            if (_isAppThread)
            {
                G.U.Err("The TimeRate for the \"Application\" time thread cannot be changed.");
                return false;
            }
            if (isTimeRateQueued && _timeRateQueued != TimeRate.Paused)
            { //TODO: re-evaluate this condition
                G.U.Warn("A new TimeRate has already been queued.");
            }
            // (comment lines added for visual symmetry)
            //
            bool isPauseProcessed = _pauseKeys.Count > 0;
            //
            if (!_pauseKeys.Contains(pauseKey))
            {
                _pauseKeys.Add(pauseKey);
            }
            if (isPauseProcessed)
            {
                //the following code has already been processed, but this is OK, so return true
                return true;
            }
            _timeRateQueued = TimeRate.Paused;
            //TODO: Make sure this happens next frame.
#if NS_DG_TWEENING
            foreach (Tween t in _tweens)
            {
                t.Pause();
            }
#endif
            return true;
        }

        public void QueuePause(object pauseKey, System.Action callback)
        {
            //TODO: what should happen to the callback if the time thread is already paused?
            if (!QueuePause(pauseKey)) return;
            _callbackPause += callback;
        }

        public bool QueueUnpause(object pauseKey)
        {
            if (_isAppThread)
            {
                G.U.Err("The TimeRate for the \"Application\" time thread cannot be changed.");
                return false;
            }
            if (isTimeRateQueued && _timeRateQueued != _timeRateUnpause)
            { //TODO: re-evaluate this condition
                G.U.Warn("A new TimeRate has already been queued.");
            }
            if (_pauseKeys.Count == 0)
            {
                // TODO: What actually happened here?
                //there is nothing left to unpause, but for functional symmetry, return true
                return false;
            }
            if (_pauseKeys.Contains(pauseKey))
            {
                _pauseKeys.Remove(pauseKey);
            }
            if (_pauseKeys.Count > 0)
            {
                //there are still pause keys locking this from being unpaused, but this is OK, so return true
                return true;
            }
            _timeRateQueued = _timeRateUnpause;
            //TODO: Make sure this happens next frame.
#if NS_DG_TWEENING
            foreach (Tween t in _tweens)
            {
                t.Play();
            }
#endif
            return true;
        }

        public void QueueUnpause(object pauseKey, System.Action callback)
        {
            //TODO: what should happen to the callback if the time thread is already unpaused?
            if (!QueueUnpause(pauseKey)) return;
            _callbackUnpause += callback;
        }

        public void QueuePauseToggle(object pauseKey)
        {
            if (!_pauseKeys.Contains(pauseKey))
            {
                QueuePause(pauseKey);
            }
            else
            {
                QueueUnpause(pauseKey);
            }
        }

        public void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int timeRatePauseKey = -1)
        {
            if (_isAppThread)
            {
                G.U.Err("The TimeRate for the \"Application\" time thread cannot be changed.");
                return;
            }
            if (isTimeRateQueued)
            {
                G.U.Err("A new TimeRate has already been queued.");
                return;
            }
            switch (timeRate)
            {
                case TimeRate.Paused:
                    QueuePause(timeRatePauseKey);
                    break;
                case TimeRate.Scaled:
                    _timeRateUnpause = timeRate;
                    _timeScale = timeScale;
                    QueueUnpause(timeRatePauseKey);
                    break;
                case TimeRate.Unscaled:
                    _timeRateUnpause = timeRate;
                    QueueUnpause(timeRatePauseKey);
                    break;
                default:
                    G.U.Unsupported(this, _timeRate);
                    break;
            }
        }

        // PRIVATE MISC METHODS

        void CheckFreeze()
        {
            if (_freezePauseKey.HasValue)
            {
                if (_freezeTime.Ap(0))
                {
                    QueueUnpause(_freezePauseKey.Value);
                    _freezePauseKey = null;
                }
                else
                {
                    _freezeTime = Mathf.Max(0, _freezeTime - Time.fixedUnscaledDeltaTime);
                }
            }
        }

        void CheckTimeRateQueued()
        {
            if (!isTimeRateQueued) return;
            switch (_timeRateQueued)
            {
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

        void Pause()
        {
            _timeRate = TimeRate.Paused;
            /*
			foreach (Sequence s in _tweens) {
				s.Pause();
			}
			*/
            ObjectManager.InvokeEventActions(ref _callbackPause, true);
            InvokePauseHandlers();
        }

        void Unpause()
        {
            _timeRate = _timeRateUnpause;
            /*
			foreach (Sequence s in _tweens) {
				s.Play();
			}
			*/
            ObjectManager.InvokeEventActions(ref _callbackUnpause, true);
            InvokeUnpauseHandlers();
        }

        // PUBLIC TRIGGER METHODS

        /// <summary>
        /// Adds a time trigger to this time thread.
        /// </summary>
        /// <returns>The TimeTrigger that was added.</returns>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        /// <param name="handler">HANDLER to be called at the end of the interval.</param>
        /// <param name="disallowFacade">If set to <c>true</c> disallow use of a time trigger facade.</param>
        public TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false)
        {
            if (iv > 0)
            {
                TimeTrigger tt = new TimeTrigger(this, iv);
                _triggersNew.Add(tt);
                tt.AddHandler(handler);
                return tt;
            }
            else if (disallowFacade)
            {
                G.U.Err("The trigger's interval must be greater than zero.");
                return null;
            }
            else if (iv.Ap(0))
            {
                //if there is no measurable interval, call the handler immediately with a time trigger facade
                TimeTriggerFacade ttfc = new TimeTriggerFacade(this);
                handler(ttfc);
                return ttfc;
            }
            else
            {
                G.U.Err("The trigger's interval must be greater than or equal to zero.");
                return null;
            }
        }

        /// <summary>
        /// Links a time trigger to this time thread.
        /// </summary>
        /// <param name="tt">The TimeTrigger to be linked.</param>
        public void LinkTrigger(TimeTrigger tt)
        {
            if (tt.totalInterval > 0)
            {
                _triggersNew.Add(tt);
            }
            else
            {
                G.U.Err("The trigger's interval must be greater than zero.");
            }
        }

        /// <summary>
        /// Removes a time trigger from this time thread. It will be disposed and unusable.
        /// </summary>
        /// <returns><c>true</c>, if trigger was removed, <c>false</c> otherwise.</returns>
        /// <param name="tt">The TimeTrigger to be removed.</param>
        public bool RemoveTrigger(TimeTrigger tt)
        {
            if (_triggers.Contains(tt) && !tt.isDisposed)
            {
                tt.Dispose();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Unlinks a time trigger from this time thread. It will still be usable for later.
        /// </summary>
        /// <returns><c>true</c>, if trigger was unlinked, <c>false</c> otherwise.</returns>
        /// <param name="tt">The TimeTrigger to be unlinked.</param>
        public bool UnlinkTrigger(TimeTrigger tt)
        {
            if (_triggers.Contains(tt) && !_triggersOut.Contains(tt))
            {
                _triggersOut.Add(tt);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Passes in a trigger reference, disposes of it (if exists), then assigns it a new trigger (AddTrigger).
        /// </summary>
        /// <param name="tt">The TimeTrigger reference to be operated upon.</param>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        /// <param name="handler">HANDLER to be called at the end of the interval.</param>
        /// <param name="disallowFacade">If set to <c>true</c> disallow use of a time trigger facade.</param>
        public void trigger(ref TimeTrigger tt, float iv, TimeTriggerHandler handler, bool disallowFacade = false)
        {
            if (tt != null) tt.Dispose();
            tt = AddTrigger(iv, handler, disallowFacade);
        }

        // PRIVATE TRIGGER METHODS

        void UpdateTriggers()
        {
            IntegrateTriggers(); //Coming from regular Update to UpdateTriggers.
            for (int i = 0; i < _triggers.Count; i++)
            {
                _triggers[i].Update(deltaTime);
                if (_triggers[i].isDisposed)
                {
                    _triggers.RemoveAt(i--);
                }
            }
            IntegrateTriggers(); //Coming from UpdateTriggers back to regular Update.
        }

        void IntegrateTriggers()
        {
            if (_triggersNew.Count > 0)
            {
                _triggers.AddRange(_triggersNew);
                _triggersNew.Clear();
                _triggers.Sort();
            }
            if (_triggersOut.Count > 0)
            {
                _triggers.RemoveAll(_triggersOut.Contains);
                _triggersOut.Clear();
                _triggers.Sort();
            }
        }

#if NS_DG_TWEENING

        // PUBLIC TWEEN METHODS (OLD)

        /// <summary>
        /// Consider using Tween(...) & Untween(...) instead.
        /// </summary>
        public void AddTween(Tween t)
        {
            _tweens.Add(t.SetUpdate(UpdateType.Fixed));
        }

        /// <summary>
        /// Consider using Tween(...) & Untween(...) instead.
        /// </summary>
        public void RemoveTween(Tween t)
        {
            _tweens.Remove(t);
        }

        // PUBLIC TWEEN METHODS (NEW)

        public void Tween(ref Tween t_ref, Tween t)
        {
            Untween(ref t_ref);
            t_ref = t;
            _tweens.Add(t.SetUpdate(UpdateType.Fixed));
        }
        public void Tween(ref Tweener t_ref, Tweener t)
        {
            Untween(ref t_ref);
            t_ref = t;
            _tweens.Add(t.SetUpdate(UpdateType.Fixed));
        }

        public void Untween(ref Tween t_ref)
        {
            if (t_ref == null) return;
            _tweens.Remove(t_ref);
            t_ref.Kill();
            t_ref = null;
        }
        public void Untween(ref Tweener t_ref)
        {
            if (t_ref == null) return;
            _tweens.Remove(t_ref);
            t_ref.Kill();
            t_ref = null;
        }

#endif

        // PUBLIC PAUSE METHODS (NEW)

        /// <summary>
        /// Pause this time thread.
        /// Must wait at least one frame to ensure safe execution.
        /// </summary>
        /// <param name="pausingObject">Usually the object calling this function.</param>
        /// <returns>The IEnumerator to be used for wait time in a coroutine.</returns>
        public IEnumerator Pause(object pausingObject)
        {
            QueuePause(pausingObject);
            yield return null;
        }

        /// <summary>
        /// Unpause this time thread.
        /// Must wait at least one frame to ensure safe execution.
        /// </summary>
        /// <param name="unpausingObject">Usually the object calling this function.</param>
        /// <returns>The IEnumerator to be used for wait time in a coroutine.</returns>
        public IEnumerator Unpause(object unpausingObject)
        {
            QueueUnpause(unpausingObject);
            yield return null;
        }
    }
}