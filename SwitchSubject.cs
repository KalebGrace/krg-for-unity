using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public class SwitchSubject : MonoBehaviour
    {
        public List<Switch> enableChainSwitches = new List<Switch>();

        protected virtual void OnEnable()
        {
            for (int i = 0; i < enableChainSwitches.Count; ++i)
            {
                enableChainSwitches[i].enabled = true;
            }
        }

        protected virtual void Update() { }
    }
}