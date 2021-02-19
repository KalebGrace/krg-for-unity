namespace _0G.Legacy
{
    public interface ILateUpdate
    {
        float priority { get; }

        void LateUpdate();
    }
}