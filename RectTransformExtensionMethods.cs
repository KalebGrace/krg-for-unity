using UnityEngine;

namespace _0G.Legacy
{
    public static class RectTransformExtensionMethods
    {
        /// <summary>
        /// Center the specified RectTransform on its parent using the specified size.
        /// This will modify the pivot, anchors, position, size, and scale.
        /// </summary>
        /// <param name="rt">RectTransform.</param>
        /// <param name="size">Size.</param>
        public static void Center(this RectTransform rt, Vector2 size)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.pivot;
            rt.anchorMax = rt.pivot;

            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = size;
            rt.localScale = Vector3.one;
        }

        /// <summary>
        /// Stretch the specified RectTransform to the full extent of its parent.
        /// This will modify the pivot, anchors, position, size, and scale.
        /// </summary>
        /// <param name="rt">RectTransform.</param>
        public static void Stretch(this RectTransform rt)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            //all three of the following are required, else irregularities arise under particular circumstances
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
            rt.localScale = Vector3.one;
        }
    }
}