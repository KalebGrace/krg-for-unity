using UnityEngine;
using System.Collections;

namespace KRG {

    /// <summary>
    /// This is a fake TimeThread class to be used when the app is shutting down and the TimeManager has been destroyed.
    /// </summary>
    public class TimeThreadFacade : ITimeThread {

        public void AddPauseHandler(System.Action handler) { }

        public void AddUnpauseHandler(System.Action handler) { }

        public void RemovePauseHandler(System.Action handler) { }

        public void RemoveUnpauseHandler(System.Action handler) { }

        public void QueueFreeze(float iv, int pauseKey = -2) { }

        public bool QueuePause(int pauseKey) { return false; }

        public void QueuePause(int pauseKey, System.Action callback) { }

        public bool QueueUnpause(int pauseKey) { return false; }

        public void QueueUnpause(int pauseKey, System.Action callback) { }

        public void QueuePauseToggle(int pauseKey) { }

        public void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int pauseKey = -1) { }

        public TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false) {
            return null;
        }

        public void LinkTrigger(TimeTrigger tt) { }

        public bool RemoveTrigger(TimeTrigger tt) { return false; }

        public bool UnlinkTrigger(TimeTrigger tt) { return false; }

#if NS_DG_TWEENING

        public void AddTween(DG.Tweening.Tween t) { }

        public void RemoveTween(DG.Tweening.Tween t) { }

#endif

        public float deltaTime { get { return 0; } }

        public float fixedDeltaTime { get { return 0; } }

        public bool isPaused { get { return true; } }

        public float speed { get { return 0; } }

        public TimeRate timeRate { get { return TimeRate.Paused; } }

        public float timeScale { get { return 1; } }
    }
}
