namespace KRG
{
    public interface IStart
    {
        float priority { get; }

        void Start();
    }
}