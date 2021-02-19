using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    public static class MenuPropagateOffsetX
    {
        [MenuItem("0G/Legacy/Propagate Offset X", false, 1001)]
        public static void PropagateOffsetX()
        {
            Transform tfRoot = Selection.activeTransform;
            float offsetX = tfRoot.position.x;

            if (!offsetX.Ap(0))
            {
                foreach (Transform tf in tfRoot.GetComponentsInChildren<Transform>(true))
                {
                    if (tf != tfRoot && tf.parent == tfRoot)
                    {
                        tf.position = tf.position.Add(offsetX);
                    }
                }

                tfRoot.position = tfRoot.position.SetX(0);
            }
        }
    }
}