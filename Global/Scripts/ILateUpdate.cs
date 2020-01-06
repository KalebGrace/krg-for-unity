namespace KRG
{
    public interface ILateUpdate
    {
        float priority { get; }

        void LateUpdate();
    }
}
