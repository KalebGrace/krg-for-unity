using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// I end.
    /// </summary>
    public interface IEnd {

        // SAMPLE IMPLEMENTATION:
        /*
#region IEnd implementation

        public End end { get; private set; }

        void Awake() {
            end = new End(this);
        }

        void OnDestroy() {
            end.InvokeActions();
        }

#endregion
        */

        End end { get; }

    }
}
