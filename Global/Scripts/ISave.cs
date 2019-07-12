namespace KRG
{
    public interface ISave
    {
        void OnSaving(ref SaveFile sf);
        void OnLoading(SaveFile sf);
    }
}
