using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IStateLogic<TOwner> where TOwner : IStateOwner {

        /// <summary>
        /// Raises the added event. Called immediately after this state has been added to
        /// the specified owner, and all processing has been completed.
        /// </summary>
        /// <param name="stateOwner">State owner.</param>
        void OnAdded(TOwner stateOwner);

        /// <summary>
        /// Raises the removed event. Called immediately after this state has been removed from
        /// the specified owner, and all processing has been completed.
        /// </summary>
        /// <param name="stateOwner">State owner.</param>
        void OnRemoved(TOwner stateOwner);

        /// <summary>
        /// Raises the renewed event. Called when this owner's state is renewed,
        /// and after all processing has been completed.
        /// </summary>
        /// <param name="stateOwner">State owner.</param>
        void OnRenewed(TOwner stateOwner);

        /// <summary>
        /// Called when this owner's state is updated.
        /// </summary>
        /// <param name="stateOwner">State owner.</param>
        void Update(TOwner stateOwner);
    }
}
