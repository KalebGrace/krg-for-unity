namespace _0G.Legacy
{
    public interface ISave
    {
        void OnSaving(SaveContext context, ref KRG.SaveFile sf);
        void OnLoading(SaveContext context, KRG.SaveFile sf);
    }
}