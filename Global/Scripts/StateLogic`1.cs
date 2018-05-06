using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public abstract class StateLogic<TOwner> : IStateLogic<TOwner> where TOwner : IStateOwner {

#region custom methods

        /// <summary>
        /// Add this state to the specified owner with special logic defined by said state.
        /// </summary>
        /// <param name="stateOwner">State owner.</param>
        public virtual void AddTo(TOwner stateOwner) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the states to lock when adding this state.
        /// </summary>
        /// <returns>The states to lock.</returns>
        /// <param name="stateOwner">State owner.</param>
        public virtual int[] GetStatesToLock(TOwner stateOwner) {
            return null;
        }

#endregion

#region IStateLogic implementation

        public virtual void OnAdded(TOwner stateOwner) {
        }

        public virtual void OnRemoved(TOwner stateOwner) {
        }

        public virtual void OnRenewed(TOwner stateOwner) {
        }

        public virtual void Update(TOwner stateOwner) {
        }

#endregion

    }
}
