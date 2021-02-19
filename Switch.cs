using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public class Switch : MonoBehaviour
    {
        // SERIALIZED FIELDS

        [SerializeField]
        protected bool m_SaveState = true;

        [SerializeField,
            Tooltip("-1 is uninitialized. 0 is the first state. 1 is the second state. Etc.")
        ]
        protected int m_StateIndex = -1;

        public List<SwitchState> states = new List<SwitchState>();

        [SerializeField]
        protected int m_ID = default;

        // PROPERTIES

        public int ID => m_ID;

        public int StateIndex => m_StateIndex;

        // MONOBEHAVIOUR METHODS

        protected virtual void OnValidate()
        {
            if (m_ID == 0)
            {
                m_ID = GetInstanceID();
            }
        }

        protected virtual void OnEnable()
        {
            if (m_SaveState)
            {
                if (G.save.GetSwitchState(this, out int stateIndex))
                {
                    m_StateIndex = stateIndex;

                    BeginState(states[m_StateIndex], true);
                }
            }
        }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }

        // CUSTOM METHODS

        public void Prev()
        {
            if (!enabled) return;

            SetStateIndex((m_StateIndex + states.Count - 1) % states.Count);
        }

        public void Next()
        {
            if (!enabled) return;

            SetStateIndex((m_StateIndex + 1) % states.Count);
        }

        public void GoTo(int stateIndex)
        {
            if (!enabled) return;

            SetStateIndex(stateIndex);
        }

        protected void SetStateIndex(int stateIndex)
        {
            m_StateIndex = stateIndex;

            if (m_SaveState)
            {
                G.save.SetSwitchState(this);
            }

            BeginState(states[m_StateIndex]);
        }

        protected void BeginState(SwitchState state, bool isInstant = false)
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
                        // do nothing
                        break;
                    case SwitchCommand.Enable:
                        SetSubjectEnabled(subject, context, true);
                        break;
                    case SwitchCommand.Disable:
                        SetSubjectEnabled(subject, context, false);
                        break;
                    case SwitchCommand.MoveTo:
                        MoveTo(subject, action.destination, isInstant);
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

        protected virtual void MoveTo(SwitchSubject subject, GameObject destination, bool isInstant)
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