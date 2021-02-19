using UnityEngine;

namespace _0G.Legacy
{
    [ExecuteInEditMode]
    public class SortingLayerExposer : MonoBehaviour
    {
        #region serialized fields

        [SerializeField]
        string _sortingLayerName = "Default";
        [SerializeField]
        int _sortingOrder = default;

        #endregion

        #region private fields

        MeshRenderer _meshRenderer;

        #endregion

        #region MonoBehaviour methods

        //WARNING: this function will only be called automatically if playing a GAME BUILD
        //...it will NOT be called if using the Unity editor
        void Awake()
        {
            UpdateMeshRenderer();
        }

        //WARNING: this function will only be called automatically if using the UNITY EDITOR
        //...it will NOT be called if playing a game build
        void OnValidate()
        {
            UpdateMeshRenderer();
        }

        #endregion

        #region private methods

        void UpdateMeshRenderer()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
                if (_meshRenderer == null)
                {
                    Debug.LogWarning("There is no MeshRenderer on this game object.");
                    return;
                }
            }
            _meshRenderer.sortingLayerName = _sortingLayerName;
            _meshRenderer.sortingOrder = _sortingOrder;
        }

        #endregion
    }
}