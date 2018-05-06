using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

#if NS_TMPRO
using TMPro;
#endif

namespace KRG {

    public static class TextMeshProUGUIExtensionMethods {

#if NS_DG_TWEENING && NS_TMPRO
        public static Tweener DOColor(this TextMeshProUGUI text, Color endValue, float duration) {
            return DOTween.To(() => text.color, x => text.color = x, endValue, duration);
        }

        public static Tweener DOFade(this TextMeshProUGUI text, float endValue, float duration) {
            return DOTween.To(() => text.alpha, x => text.alpha = x, endValue, duration);
        }
#endif
    }
}
