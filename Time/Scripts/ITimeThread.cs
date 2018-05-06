using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KRG {

    public interface ITimeThread {
        
#region Properties

        float deltaTime { get; }

        float fixedDeltaTime { get; }

        bool isPaused { get; }

        float speed { get; }

        TimeRate timeRate { get; }

        float timeScale { get; }

#endregion

#region Methods: Handler

        void AddPauseHandler(System.Action handler);

        void AddUnpauseHandler(System.Action handler);

        void RemovePauseHandler(System.Action handler);

        void RemoveUnpauseHandler(System.Action handler);

#endregion

#region Methods: Queue

        /// <summary>
        /// Queues the time freeze (freezes the time thread for the specified unscaled realtime duration).
        /// </summary>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        /// <param name="pauseKey">Pause key (optional).</param>
        void QueueFreeze(float iv, int pauseKey = -2);

        bool QueuePause(int pauseKey);

        void QueuePause(int pauseKey, System.Action callback);

        bool QueueUnpause(int pauseKey);

        void QueueUnpause(int pauseKey, System.Action callback);

        void QueuePauseToggle(int pauseKey);

        /// <summary>
        /// Queues the time rate.
        /// </summary>
        /// <param name="timeRate">Time rate.</param>
        /// <param name="timeScale">Time scale (only used with TimeRate.Scaled).</param>
        /// <param name="pauseKey">Pause key (optional).</param>
        void QueueTimeRate(TimeRate timeRate, float timeScale = 1, int pauseKey = -1);

#endregion

#region Methods: Trigger

        TimeTrigger AddTrigger(float iv, TimeTriggerHandler handler, bool disallowFacade = false);

        void LinkTrigger(TimeTrigger tt);

        bool RemoveTrigger(TimeTrigger tt);

        bool UnlinkTrigger(TimeTrigger tt);

#endregion

#if NS_DG_TWEENING
        
#region Methods: Tween

        void AddTween(DG.Tweening.Tween t);

        void RemoveTween(DG.Tweening.Tween t);

#endregion

#endif

    }
}
