using UnityEngine;

namespace KRG
{
    public interface ISpawn
    {
        Transform transform { get; } // the root transform of the spawner
        Transform CenterTransform { get; }
        GameObjectBody Invoker { get; }
    }
}
