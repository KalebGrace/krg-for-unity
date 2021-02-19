namespace _0G.Legacy
{
    public interface IStateLogic<TOwner> where TOwner : IStateOwner
    {
        /// <summary>
        /// Raises the added event. Called immediately after this state has been added to
        /// the specified owner, and all processing has been completed.
        /// </summary>
        /// <param name="owner">The IStateOwner object.</param>
        void OnAdded(TOwner owner);

        /// <summary>
        /// Raises the removed event. Called immediately after this state has been removed from
        /// the specified owner, and all processing has been completed.
        /// </summary>
        /// <param name="owner">The IStateOwner object.</param>
        void OnRemoved(TOwner owner);

        /// <summary>
        /// Raises the renewed event. Called when this owner's state is renewed,
        /// and after all processing has been completed.
        /// </summary>
        /// <param name="owner">The IStateOwner object.</param>
        void OnRenewed(TOwner owner);

        /// <summary>
        /// Called when this owner's state is updated.
        /// </summary>
        /// <param name="owner">The IStateOwner object.</param>
        void Update(TOwner owner);
    }
}