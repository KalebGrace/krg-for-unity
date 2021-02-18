namespace KRG
{
    public interface ISave
    {
        void OnSaving(SaveContext context, ref SaveFile sf);
        void OnLoading(SaveContext context, SaveFile sf);
    }
}