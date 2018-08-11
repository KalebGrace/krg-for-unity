using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public class UIManager : Manager, IUIManager {

#region fields

        protected GameObject _colorOverlay;
#if NS_DG_TWEENING
        protected Tweener _colorOverlayTweener;
#endif

#endregion

#region IManager implementation

        public override void Awake() {
        }

#endregion

#region IUIManager implementation: methods

        public virtual Image AddColorOverlay(Color color) {
            RemoveColorOverlay();

            _colorOverlay = new GameObject("ColorOverlayCanvas", typeof(Canvas));
            _colorOverlay.PersistNewScene();
            var c = _colorOverlay.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 1024;

            var go = new GameObject("ColorOverlayImage", typeof(Image));
            go.transform.SetParent(_colorOverlay.transform);
            var rt = go.GetComponent<RectTransform>();
            rt.Stretch();
            var i = go.GetComponent<Image>();
            i.color = color;

            return i;
        }

#if NS_DG_TWEENING
        public virtual Tweener FadeColorOverlay(Color color1, Color color2, float duration) {
            var i = AddColorOverlay(color1);
            _colorOverlayTweener = i.DOColor(color2, duration);
            return _colorOverlayTweener;
        }

        public virtual Tweener FadeInColorOverlay(Color color, float duration) {
            var i = AddColorOverlay(color.SetAlpha(0)); //use transparent clone of color
            _colorOverlayTweener = i.DOFade(color.a, duration);
            return _colorOverlayTweener;
        }

        public virtual Tweener FadeOutColorOverlay(Color color, float duration) {
            var i = AddColorOverlay(color);
            _colorOverlayTweener = i.DOFade(0, duration);
            return _colorOverlayTweener;
        }
#endif

        public virtual void RemoveColorOverlay() {
#if NS_DG_TWEENING
            if (_colorOverlayTweener != null) {
                _colorOverlayTweener.Kill();
                _colorOverlayTweener = null;
            }
#endif
            if (_colorOverlay != null) {
                Object.Destroy(_colorOverlay);
                _colorOverlay = null;
            }
        }

#endregion

    }
}
