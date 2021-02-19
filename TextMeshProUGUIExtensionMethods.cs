using TMPro;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public static class TextMeshProUGUIExtensionMethods
    {
#if NS_DG_TWEENING
        public static Tweener DOColor(this TextMeshProUGUI text, Color endValue, float duration)
        {
            return DOTween.To(() => text.color, x => text.color = x, endValue, duration);
        }

        public static Tweener DOFade(this TextMeshProUGUI text, float endValue, float duration)
        {
            return DOTween.To(() => text.alpha, x => text.alpha = x, endValue, duration);
        }

        public static Tweener DOFontSize(this TextMeshProUGUI text, float endValue, float duration)
        {
            return DOTween.To(() => text.fontSize, x => text.fontSize = x, endValue, duration);
        }
#endif
    }
}