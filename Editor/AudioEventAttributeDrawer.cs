using UnityEditor;

namespace _0G.Legacy
{
    [CustomPropertyDrawer(typeof(AudioEventAttribute))]
#if NS_FMOD
    public class AudioEventAttributeDrawer : FMODUnity.EventRefDrawer { } // class must be made public
#else
    public class AudioEventAttributeDrawer : PropertyDrawer { }
#endif
}