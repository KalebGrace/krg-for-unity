using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KRG {

    public interface IPrefab {

        Dictionary<string, Transform> prefabs { get; }

        void OnPrefabAdd(string key, Transform instance);
    }
}
