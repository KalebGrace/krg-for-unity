namespace _0G.Legacy
{
    public interface IOnApplicationQuit
    {
        float priority { get; }

        void OnApplicationQuit();
    }
}