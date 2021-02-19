using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public class TimeTrigger : System.IComparable<TimeTrigger>
    {
        //If true, this will fire multiple times if trigger is pulled more than once over delta.
        public virtual bool doesMultiFire { get; set; }

        public int intData { get; set; }

        public bool isDisposed { get; private set; }

        public bool isFrozen { get; private set; }

        public double timeElapsed { get; private set; }

        public float timeRemaining { get; private set; }

        public TimeThread timeThread { get; private set; }

        public float totalInterval { get; private set; }

        List<TimeTriggerHandler> _handlers = new List<TimeTriggerHandler>();
        MultiFireState _multiFireState = MultiFireState.Running;

        public enum MultiFireState
        {
            Running,
            RestartWait,
            ProceedWait,
            Finishing,
        }

        /// <summary>
        /// Constructor for TimeTriggerFacade.
        /// </summary>
        /// <param name="th">Linked TimeTHREAD.</param>
        protected TimeTrigger(TimeThread th)
        {
            timeThread = th;
        }

        /// <summary>
        /// Constructor for TimeTrigger.
        /// </summary>
        /// <param name="th">Linked TimeTHREAD.</param>
        /// <param name="iv">Time INTERVAL (in seconds).</param>
        public TimeTrigger(TimeThread th, float iv)
        {
            G.U.Assert(iv > 0);
            timeRemaining = iv;
            timeThread = th;
            totalInterval = iv;
        }

        public int CompareTo(TimeTrigger tt)
        {
            if (tt != null)
            {
                return this.timeRemaining.CompareTo(tt.timeRemaining);
            }
            else
            {
                return 1;
            }
        }

        public virtual void AddHandler(TimeTriggerHandler handler)
        {
            _handlers.Add(handler);
        }

        public virtual bool RemoveHandler(TimeTriggerHandler handler)
        {
            return _handlers.Remove(handler);
        }

        public virtual bool HasHandler(TimeTriggerHandler handler)
        {
            return _handlers.Contains(handler);
        }

        /// <summary>
        /// Pull the trigger immediately.
        /// </summary>
        public virtual void Trigger()
        {
            Update(totalInterval, false);
        }

        /// <summary>
        /// Advance time.
        /// </summary>
        /// <param name="delta">The time delta.</param>
        public virtual void Update(float delta)
        {
            Update(delta, true);
        }

        void Update(float delta, bool updatesTimeElapsed)
        {
            if (isFrozen) return;

            if (updatesTimeElapsed)
            {
                timeElapsed += delta;
            }

            if (timeRemaining <= 0)
            {
                //The trigger was previously pulled,
                //but nothing was done to reset this object.
                //We will simply dispose of this object to be cautious.
                Dispose();
                return;
            }

            timeRemaining -= delta;

            if (timeRemaining > 0)
            {
                //Time passed, but the trigger was not pulled.
                return;
            }

            if (doesMultiFire)
            {
                MultiFireEvent();
            }
            else
            {
                //Simply pull the trigger once.
                FireEvent();
            }
        }

        void MultiFireEvent()
        {
            //Calculate the number of "shots".
            int i = 1 + Mathf.FloorToInt(Mathf.Abs(timeRemaining) / totalInterval);

            while (i > 0 && doesMultiFire && !isDisposed)
            { //doesMultiFire & isDisposed could be changed by the fired event.
                FireEvent();
                i--;
            }

            if (isDisposed)
            {
                return;
            }

            switch (_multiFireState)
            {
                case MultiFireState.RestartWait:
                    _multiFireState = MultiFireState.Finishing;
                    Restart();
                    break;
                case MultiFireState.ProceedWait:
                    _multiFireState = MultiFireState.Finishing;
                    Proceed();
                    break;
                default:
                    //This trigger is obviously no longer needed.
                    break;
            }

            _multiFireState = MultiFireState.Running;
        }

        void FireEvent()
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                if (isDisposed)
                { //isDisposed could be changed by the fired event.
                    break;
                }
                if (_handlers[i] == null)
                { //The object that contains the handler could no longer exist.
                    _handlers.RemoveAt(i--);
                    continue;
                }
                _handlers[i](this);
            }
        }

        /// <summary>
        /// Re-activate the TimeTrigger, while maintaining continuity in the cycle.
        /// </summary>
        public virtual void Proceed()
        {
            if (doesMultiFire)
            {
                switch (_multiFireState)
                {
                    case MultiFireState.Running:
                        _multiFireState = MultiFireState.ProceedWait;
                        return;
                    case MultiFireState.Finishing:
                        break; //Pass through.
                    default:
                        return;
                }
            }
            while (timeRemaining <= 0)
            {
                timeRemaining += totalInterval;
            }
        }

        /// <summary>
        /// Re-activate the TimeTrigger, and start from the beginning of the cycle.
        /// </summary>
        public virtual void Restart()
        {
            if (doesMultiFire)
            {
                switch (_multiFireState)
                {
                    case MultiFireState.Running:
                    case MultiFireState.ProceedWait:
                        _multiFireState = MultiFireState.RestartWait;
                        return;
                    case MultiFireState.Finishing:
                        break; //Pass through.
                    default:
                        return;
                }
            }
            timeRemaining = totalInterval;
        }

        /// <summary>
        /// If true, freezes the TimeTrigger, causing no time to elapse until unfrozen.
        /// Trigger() and Update(...) will not work while the TimeTrigger is frozen.
        /// If false, unfreezes the TimeTrigger (or "thaws" it, if you prefer).
        /// </summary>
        public void Freeze(bool value)
        {
            isFrozen = value;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="TimeTrigger"/> object.
        /// This stops all handlers WITHOUT triggering them.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="TimeTrigger"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="TimeTrigger"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the <see cref="TimeTrigger"/> so the garbage
        /// collector can reclaim the memory that the <see cref="TimeTrigger"/> was occupying.</remarks>
        public void Dispose()
        {
            timeRemaining = 0f;
            _handlers.Clear();
            isDisposed = true;
        }
    }
}