namespace KRG
{
    public interface IOnApplicationQuit
    {
        float priority { get; }

        void OnApplicationQuit();
    }
}
