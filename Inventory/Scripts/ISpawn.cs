using UnityEngine;

namespace KRG
{
    public interface ISpawn
    {
        Transform centerTransform { get; }
        Transform transform { get; }
    }
}
