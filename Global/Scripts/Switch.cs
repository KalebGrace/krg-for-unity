using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class Switch : MonoBehaviour
    {
        [SerializeField]
        protected int m_StateIndex = -1; //TODO: base 0 tooltip
        //0 is the first state. 1 is the second state. And so on...

        public List<SwitchState> states = new List<SwitchState>();

        public int StateIndex => m_StateIndex;

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Update()
        {
        }

        public void Prev()
        {
            if (!enabled) return;

            m_StateIndex = (m_StateIndex + states.Count - 1) % states.Count;

            BeginState(states[m_StateIndex]);
        }

        public void Next()
        {
            if (!enabled) return;

            m_StateIndex = (m_StateIndex + 1) % states.Count;

            BeginState(states[m_StateIndex]);
        }

        public void GoTo(int stateIndex)
        {
            if (!enabled) return;

            m_StateIndex = stateIndex;

            BeginState(states[m_StateIndex]);
        }

        protected void BeginState(SwitchState state)
        {
            for (int i = 0; i < state.actions.Count; ++i)
            {
                var action = state.actions[i];
                var subject = action.subject;
                var command = action.command;
                var context = action.context;

                switch (command)
                {
                    case SwitchCommand.None:
                        //do nothing
                        break;
                    case SwitchCommand.Enable:
                        SetSubjectEnabled(subject, context, true);
                        break;
                    case SwitchCommand.Disable:
                        SetSubjectEnabled(subject, context, false);
                        break;
                    case SwitchCommand.MoveTo:
                        MoveTo(subject, action.destination);
                        break;
                    case SwitchCommand.StatePrev:
                    case SwitchCommand.StateNext:
                    case SwitchCommand.StateGoTo:
                        SetSubjectState(subject, command, context, action.index);
                        break;
                    case SwitchCommand.SetGameObjectActive:
                        SetSubjectGameObjectActive(subject, true);
                        break;
                    case SwitchCommand.SetGameObjectInactive:
                        SetSubjectGameObjectActive(subject, false);
                        break;
                    default:
                        G.U.Unsupported(this, command);
                        break;
                }
            }
        }

        protected void SetSubjectEnabled(SwitchSubject subject, SwitchContext context, bool enabled)
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

        protected void SetSubjectState(SwitchSubject subject, SwitchCommand command, SwitchContext context, int stateIndex)
        {
            Switch component;

            switch (context)
            {
                case SwitchContext.Switch:
                    component = subject.GetComponent<Switch>();
                    break;
                default:
                    G.U.Unsupported(this, context);
                    return;
            }

            switch (command)
            {
                case SwitchCommand.StatePrev:
                    component.Prev();
                    break;
                case SwitchCommand.StateNext:
                    component.Next();
                    break;
                case SwitchCommand.StateGoTo:
                    component.GoTo(stateIndex);
                    break;
                default:
                    G.U.Unsupported(this, command);
                    break;
            }
        }

        protected void SetSubjectGameObjectActive(SwitchSubject subject, bool value)
        {
            subject.gameObject.SetActive(value);
        }
    }
}
