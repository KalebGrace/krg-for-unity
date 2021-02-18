namespace KRG
{
    public interface IUpdate
    {
        float priority { get; }

        void Update();
    }
}