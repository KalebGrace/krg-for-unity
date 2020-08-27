using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public interface IPrefab
    {
        Dictionary<string, Transform> prefabs { get; }

        void OnPrefabAdd(string key, Transform instance);
    }
}