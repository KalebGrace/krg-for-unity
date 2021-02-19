using UnityEngine;

namespace _0G.Legacy
{
    public class MeshSortingLayer : MonoBehaviour
    {
        private const float MAGNITUDE = 0.001f;

        private bool _init;
        private Axis _initAxis;
        private Sort _initSort;
        private string _initSortingLayerName;

        public void Init(
            string sortingLayerName = SortingLayerName.Default,
            Axis axis = Axis.Zneg,
            Sort sort = Sort.Default
        )
        {
            if (!IsValid(sortingLayerName) || !IsValid(sort)) return;
            _initSortingLayerName = sortingLayerName;
            _initAxis = axis;
            _initSort = sort;
            _init = true;
            Set(sortingLayerName, axis, sort);
        }

        public void Revert()
        {
            Set(_initSortingLayerName, _initAxis, _initSort);
        }

        public void Set(
            string sortingLayerName = SortingLayerName.Default,
            Axis axis = Axis.Zneg,
            Sort sort = Sort.Default
        )
        {
            if (!IsInitialized()) return;
            if (!IsValid(sortingLayerName) || !IsValid(sort)) return;
            int value = SortingLayer.GetLayerValueFromName(sortingLayerName);
            if (sort == Sort.Reverse) value = SortingLayer.layers.Length - value - 1;
            transform.localPosition = axis.GetVector3(value * MAGNITUDE, transform.localPosition);
        }

        private bool IsInitialized()
        {
            return G.U.Assert(_init, "Call Init first.");
        }

        private static bool IsValid(string sortingLayerName)
        {
            if (sortingLayerName == SortingLayerName.Default)
            {
                return true;
            }
            return G.U.Assert(SortingLayer.NameToID(sortingLayerName) != 0,
                "Sorting layer name " + sortingLayerName + " is invalid.");
        }

        private static bool IsValid(Sort sort)
        {
            return G.U.Assert(sort == Sort.Default || sort == Sort.Reverse);
        }
    }
}