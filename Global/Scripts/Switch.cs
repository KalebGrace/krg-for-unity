using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class Switch : MonoBehaviour
    {
        [SerializeField]
        protected int m_StateIndex = 0; //TODO: base 0 tooltip
        //0 is the first state. 1 is the second state. And so on...

        public List<SwitchState> states = new List<SwitchState>();

        public int StateIndex => m_StateIndex;

        protected virtual void Update()
        {
        }

        public void GoTo(int stateIndex)
        {
            if (!enabled) return;

            m_StateIndex = stateIndex;

            BeginState(states[m_StateIndex]);
        }

        public void Next()
        {
            if (!enabled) return;

            m_StateIndex = (m_StateIndex + 1) % states.Count;

            BeginState(states[m_StateIndex]);
        }

        protected void BeginState(SwitchState state)
        {
            for (int i = 0; i < state.actions.Count; ++i)
            {
                var action = state.actions[i];

                switch (action.command)
                {
                    case SwitchCommand.None:
                        //do nothing
                        break;
                    case SwitchCommand.Enable:
                        SetEnabled(action.subject, action.context, true);
                        break;
                    case SwitchCommand.Disable:
                        SetEnabled(action.subject, action.context, false);
                        break;
                    case SwitchCommand.MoveTo:
                        MoveTo(action.subject, action.destination);
                        break;
                    default:
                        G.U.Unsupported(this, action.command);
                        break;
                }
            }
        }

        protected void SetEnabled(SwitchSubject subject, SwitchContext context, bool enabled)
        {
            MonoBehaviour component;

            switch (context)
            {
                case SwitchContext.Subject:
                    component = subject;
                    break;
                case SwitchContext.Switch:
                    component = subject.GetComponent<Switch>();
                    break;
                default:
                    G.U.Unsupported(this, context);
                    return;
            }

            component.enabled = enabled;
        }

        protected virtual void MoveTo(SwitchSubject subject, GameObject destination)
        {
            throw new System.Exception("Not yet implemented.");
        }
    }
}
