using UnityEngine;
using UnityEngine.UI;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG
{
    public class UIManager : Manager
    {
        public override float priority { get { return 100; } }

        protected GameObject _colorOverlay;
#if NS_DG_TWEENING
        protected Tweener _colorOverlayTweener;
#endif
        public override void Awake()
        {
        }

        /// <summary>
        /// Adds a color overlay to the active scene. This creates both a high sort order Canvas, and a child Image.
        /// </summary>
        /// <returns>The newly created Image component used to display the provided color.</returns>
        /// <param name="color">Color.</param>
        public virtual Image AddColorOverlay(Color color)
        {
            RemoveColorOverlay();

            _colorOverlay = new GameObject("ColorOverlayCanvas", typeof(Canvas));
            _colorOverlay.PersistNewScene(PersistNewSceneType.MoveToHierarchyRoot);
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
        /// <summary>
        /// Fades a color overlay from color 1 to color 2.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color1">Color 1.</param>
        /// <param name="color2">Color 2.</param>
        /// <param name="duration">Duration in seconds.</param>
        public virtual Tweener FadeColorOverlay(Color color1, Color color2, float duration)
        {
            var i = AddColorOverlay(color1);
            _colorOverlayTweener = i.DOColor(color2, duration);
            return _colorOverlayTweener;
        }

        /// <summary>
        /// Fades in a color overlay.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color">Color.</param>
        /// <param name="duration">Duration in seconds.</param>
        public virtual Tweener FadeInColorOverlay(Color color, float duration)
        {
            var i = AddColorOverlay(color.SetAlpha(0)); //use transparent clone of color
            _colorOverlayTweener = i.DOFade(color.a, duration);
            return _colorOverlayTweener;
        }

        /// <summary>
        /// Fades out a color overlay.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color">Color.</param>
        /// <param name="duration">Duration in seconds.</param>
        public virtual Tweener FadeOutColorOverlay(Color color, float duration)
        {
            var i = AddColorOverlay(color);
            _colorOverlayTweener = i.DOFade(0, duration);
            return _colorOverlayTweener;
        }
#endif

        /// <summary>
        /// Removes the color overlay from the canvas.
        /// </summary>
        public virtual void RemoveColorOverlay()
        {
#if NS_DG_TWEENING
            if (_colorOverlayTweener != null)
            {
                _colorOverlayTweener.Kill();
                _colorOverlayTweener = null;
            }
#endif
            if (_colorOverlay != null)
            {
                Object.Destroy(_colorOverlay);
                _colorOverlay = null;
            }
        }
    }
}
