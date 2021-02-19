namespace _0G.Legacy
{
    /// <summary>
    /// This is a typical base class implementation of IStateLogic.
    /// You may either derive from this, or use your own implementation of IStateLogic.
    /// </summary>
    public abstract class StateLogicBase<TOwner> : IStateLogic<TOwner> where TOwner : IStateOwner
    {
        /// <summary>
        /// Add this state to the specified owner with special logic defined by said state.
        /// </summary>
        /// <param name="owner">The IStateOwner object.</param>
        public virtual void AddTo(TOwner owner)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the states to lock when adding this state.
        /// </summary>
        /// <returns>The states to lock.</returns>
        /// <param name="owner">The IStateOwner object.</param>
        public virtual int[] GetStatesToLock(TOwner owner)
        {
            return null;
        }

        // IStateLogic implementation

        public virtual void OnAdded(TOwner owner) { }

        public virtual void OnRemoved(TOwner owner) { }

        public virtual void OnRenewed(TOwner owner) { }

        public virtual void Update(TOwner owner) { }
    }
}