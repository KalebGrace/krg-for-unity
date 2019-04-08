namespace KRG
{
    public interface IOnDestroy
    {
        float priority { get; }

        void OnDestroy();
    }
}
