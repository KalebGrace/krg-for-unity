using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public interface IPrefab
    {
        Dictionary<string, Transform> prefabs { get; }

        void OnPrefabAdd(string key, Transform instance);
    }
}