namespace KRG
{
    public interface ISaveComplete : ISave
    {
        void OnSavingCompleted(SaveContext context, SaveFile sf);
        void OnLoadingCompleted(SaveContext context, SaveFile sf);
    }
}