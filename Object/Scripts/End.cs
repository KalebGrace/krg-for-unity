using System.Collections;
using System.Collections.Generic;
using KRG;
using UnityEngine;

namespace KRG {

    public class End {

        public event System.Action actions;

        public bool isEnded { get; private set; }

        public MonoBehaviour owner { get; private set; }

        public End(MonoBehaviour owner) {
            this.owner = owner;
        }

        /// <summary>
        /// Invokes the end actions. Should only be called by the owner class.
        /// </summary>
        public void InvokeActions() {
            if (actions != null) actions();
        }

        /// <summary>
        /// Marks the owner as ended. Should only be called by G.End(...).
        /// </summary>
        public void MarkAsEnded() {
            isEnded = true;
        }
    }
}
