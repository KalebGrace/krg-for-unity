namespace KRG
{
    public interface ISaveComplete : ISave
    {
        void OnSavingCompleted(SaveFile sf);
        void OnLoadingCompleted(SaveFile sf);
    }
}