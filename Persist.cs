namespace _0G.Legacy
{
    public enum Persist
    {
        None = 0,
        NewScene = 10,
        PlayerPrefs = 20, // state id 0
        PlayerPrefs_Overwrite = 22, // ditto
        EasySave3 = 30, // state id: 0 undefined, <0 auto slot, >0 manual slot
        //
        KRGConfig = 70,
    }
}