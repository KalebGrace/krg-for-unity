namespace _0G.Legacy
{
    /// <summary>
    /// IOnDestroy is intended for implementation by Manager classes.
    /// </summary>
    public interface IOnDestroy
    {
        // IMPORTANT: priority for this interface will be reversed
        float priority { get; }

        void OnDestroy();
    }
}