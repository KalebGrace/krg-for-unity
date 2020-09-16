#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    /// <summary>
    /// This is a fake TimeThread class to be used when the app is shutting down and the TimeManager has been destroyed.
    /// </summary>
    public class TimeThreadFacade : TimeThread
    {
        public TimeThreadFacade(int index = 0) : base(index) { }

        new public void AddPauseHandler(System.Action handler) { }

        new public void AddUnpauseHandler(System.Action handler) { }

        new public void RemovePauseHandler(System.Action handler) { }

        new public void RemoveUnpauseHandler(System.Action handler) { }

        new public void QueueFreeze(float iv, int pauseKey = -2) { }

        new public bool QueuePause(int pauseKey) { return false; }

        new public void QueuePause(int pauseKey, System.Action callback) { }

        new public bool QueueUnpause(int pauseKey) { return false; }

        new public void QueueUnpause(int pauseKey, System.Action callback) { }

        new public void QueuePauseToggle(int pauseKey) { }

        new public void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int pauseKey = -1) { }

        new public TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false) { return null; }

        new public void LinkTrigger(TimeTrigger tt) { }

        new public bool RemoveTrigger(TimeTrigger tt) { return false; }

        new public bool UnlinkTrigger(TimeTrigger tt) { return false; }

        new public void trigger(ref TimeTrigger tt, float iv, TimeTriggerHandler handler, bool disallowFacade = false) { }

#if NS_DG_TWEENING

        new public void AddTween(Tween t) { }

        new public void RemoveTween(Tween t) { }

        new public void Tween(ref Tween t_ref, Tween t) { }
        new public void Tween(ref Tweener t_ref, Tweener t) { }

        new public void Untween(ref Tween t_ref) { }
        new public void Untween(ref Tweener t_ref) { }

#endif

        new public float deltaTime => 0;

        new public float fixedDeltaTime => 0;

        new public bool isPaused => true;

        new public float speed => 0;

        new public TimeRate timeRate => TimeRate.Paused;

        new public float timeScale => 1;
    }
}