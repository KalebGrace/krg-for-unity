namespace KRG
{
    public interface IOnDestroy
    {
        // IMPORTANT: priority for this interface will be reversed
        float priority { get; }

        void OnDestroy();
    }
}
