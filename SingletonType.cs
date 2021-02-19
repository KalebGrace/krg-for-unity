namespace _0G.Legacy
{
    /// <summary>
    /// Singleton type.
    /// See ISingletonComponent for usage.
    /// </summary>
    public enum SingletonType
    {
        //not a singleton; any number of instances can exist
        None = 0,

        //singleton: only uses the first instance and considers all additionals to be duplicates
        SingletonFirstOnly = 1,

        //singleton: always uses the newest instance and considers any old ones to be duplicates
        SingletonAlwaysNew = 2,
    }
}