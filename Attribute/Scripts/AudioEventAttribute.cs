namespace KRG
{
#if NS_FMOD
    public class AudioEventAttribute : FMODUnity.EventRefAttribute { }
#else
    public class AudioEventAttribute : UnityEngine.PropertyAttribute { }
#endif
}
