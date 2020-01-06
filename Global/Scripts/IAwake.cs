namespace KRG
{
    public interface IAwake
    {
        float priority { get; }

        void Awake();
    }
}
