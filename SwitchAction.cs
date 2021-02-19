using UnityEngine;

namespace _0G.Legacy
{
    [System.Serializable]
    public struct SwitchAction
    {
        public SwitchSubject subject;

        public SwitchCommand command;

        //for the Behaviour to Enable/Disable
        // or the component State to modify
        public SwitchContext context;

        //for the MoveTo command only
        public GameObject destination;

        //for the StateGoTo command only
        public int index;
    }
}