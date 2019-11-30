﻿using System;

namespace KRG
{
    public struct EventAction
    {
        private event Action ActionHigh;
        private event Action ActionNormal;

        public void AddHigh(Action action)
        {
            if (ActionHigh != null)
            {
                G.U.Warn("There is already a high priority action for this event.");
            }
            ActionHigh += action;
        }

        public static EventAction operator +(EventAction eventAction, Action action)
        {
            eventAction.ActionNormal += action;
            return eventAction;
        }

        public static EventAction operator -(EventAction eventAction, Action action)
        {
            eventAction.ActionHigh -= action;
            eventAction.ActionNormal -= action;
            return eventAction;
        }

        public void Invoke()
        {
            ActionHigh?.Invoke();
            ActionNormal?.Invoke();
        }
    }
}
