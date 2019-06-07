namespace KRG
{
    public interface ISave
    {
        void SaveTo(ref SaveFile sf);

        void LoadFrom(SaveFile sf);
    }
}
