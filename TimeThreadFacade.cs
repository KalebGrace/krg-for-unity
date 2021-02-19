using System.Collections;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    /// <summary>
    /// This is a fake TimeThread class to be used when the app is shutting down and the TimeManager has been destroyed.
    /// </summary>
    public class TimeThreadFacade : TimeThread
    {
        // PUBLIC PROPERTIES

        new public float deltaTime => 0;

        new public float fixedDeltaTime => 0;

        new public bool isPaused => true;

        new public float speed => 0;

        new public TimeRate timeRate => TimeRate.Paused;

        new public float timeScale => 1;

        // CONSTRUCTOR

        public TimeThreadFacade(int index = 0) : base(index) { }

        // PUBLIC MONOBEHAVIOUR-LIKE METHODS

        new public void FixedUpdate() { }

        // PUBLIC HANDLER METHODS

        new public void AddPauseHandler(System.Action handler) { }

        new public void AddUnpauseHandler(System.Action handler) { }

        new public void RemovePauseHandler(System.Action handler) { }

        new public void RemoveUnpauseHandler(System.Action handler) { }

        // PUBLIC QUEUE METHODS

        new public void QueueFreeze(float iv, int freezePauseKey = -2) { }

        new public bool QueuePause(object pauseKey) { return false; }

        new public void QueuePause(object pauseKey, System.Action callback) { }

        new public bool QueueUnpause(object pauseKey) { return false; }

        new public void QueueUnpause(object pauseKey, System.Action callback) { }

        new public void QueuePauseToggle(object pauseKey) { }

        new public void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int timeRatePauseKey = -1) { }

        // PUBLIC TRIGGER METHODS

        new public TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false) { return null; }

        new public void LinkTrigger(TimeTrigger tt) { }

        new public bool RemoveTrigger(TimeTrigger tt) { return false; }

        new public bool UnlinkTrigger(TimeTrigger tt) { return false; }

        new public void trigger(ref TimeTrigger tt, float iv, TimeTriggerHandler handler, bool disallowFacade = false) { }

#if NS_DG_TWEENING

        // PUBLIC TWEEN METHODS (OLD)

        [System.Obsolete]
        new public void AddTween(Tween t) { }

        [System.Obsolete]
        new public void RemoveTween(Tween t) { }

        // PUBLIC TWEEN METHODS (NEW)

        new public void Tween(ref Tween t_ref, Tween t) { }
        new public void Tween(ref Tweener t_ref, Tweener t) { }

        new public void Untween(ref Tween t_ref) { }
        new public void Untween(ref Tweener t_ref) { }

#endif

        // PUBLIC PAUSE METHODS (NEW)

        new public IEnumerator Pause(object pausingObject) { yield return null; }

        new public IEnumerator Unpause(object unpausingObject) { yield return null; }
    }
}