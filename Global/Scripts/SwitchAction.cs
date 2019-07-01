using UnityEngine;

namespace KRG
{
    [System.Serializable]
    public struct SwitchAction
    {
        public SwitchSubject subject;

        public SwitchCommand command;

        public SwitchContext context; //for the Behaviour to Enable/Disable

        public GameObject destination; //for the MoveTo command only
    }
}
