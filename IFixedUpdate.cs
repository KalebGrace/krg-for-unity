namespace _0G.Legacy
{
    public interface IFixedUpdate
    {
        float priority { get; }

        void FixedUpdate();
    }
}