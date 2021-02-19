namespace _0G.Legacy
{
    public interface ISaveComplete : ISave
    {
        void OnSavingCompleted(SaveContext context, KRG.SaveFile sf);
        void OnLoadingCompleted(SaveContext context, KRG.SaveFile sf);
    }
}