using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public class MeshSortingLayer : MonoBehaviour {

#region constants

        const float _magnitude = 0.001f;

#endregion

#region fields

        bool _init;
        Axis _initAxis;
        Sort _initSort;
        string _initSortingLayerName;
        Transform _transform;

#endregion

#region MonoBehaviour methods

        void Awake() {
            _transform = transform;
        }

#endregion

#region public methods

        public void Init(
            string sortingLayerName = SortingLayerName.Default,
            Axis axis = Axis.Zneg,
            Sort sort = Sort.Default
        ) {
            if (!IsValid(sortingLayerName) || !IsValid(sort)) return;
            _initSortingLayerName = sortingLayerName;
            _initAxis = axis;
            _initSort = sort;
            _init = true;
            Set(sortingLayerName, axis, sort);
        }

        public void Revert() {
            Set(_initSortingLayerName, _initAxis, _initSort);
        }

        public void Set(
            string sortingLayerName = SortingLayerName.Default,
            Axis axis = Axis.Zneg,
            Sort sort = Sort.Default
        ) {
            if (!IsInitialized()) return;
            if (!IsValid(sortingLayerName) || !IsValid(sort)) return;
            int value = SortingLayer.GetLayerValueFromName(sortingLayerName);
            if (sort == Sort.Reverse) value = SortingLayer.layers.Length - value - 1;
            _transform.localPosition = axis.GetVector3((float)value * _magnitude, _transform.localPosition);
        }

#endregion

#region private methods

        bool IsInitialized() {
            return G.U.Assert(_init, "Call Init first.");
        }

        static bool IsValid(string sortingLayerName) {
            return G.U.Assert(SortingLayer.NameToID(sortingLayerName) != 0, "Sorting layer name is invalid.");
        }

        static bool IsValid(Sort sort) {
            return G.U.Assert(sort == Sort.Default || sort == Sort.Reverse);
        }

#endregion

    }
}
