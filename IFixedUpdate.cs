namespace KRG
{
    public interface IFixedUpdate
    {
        float priority { get; }

        void FixedUpdate();
    }
}