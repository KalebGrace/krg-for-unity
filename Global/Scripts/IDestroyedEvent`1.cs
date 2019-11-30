namespace KRG
{
    public interface IDestroyedEvent<T>
    {
        event System.Action<T> Destroyed;
    }
}
