namespace KRG
{
    public interface IStateOwner
    {
        void AddStateHandler(ulong state, StateHandler handler);
        void RemoveStateHandler(ulong state, StateHandler handler);

        bool HasState(ulong state);
    }
}