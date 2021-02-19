using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace _0G.Legacy
{
    public class UIManager : Manager
    {
        public override float priority => 100;

        // FIELDS

        protected GameObject _colorOverlay;
        protected bool _colorOverlayIsLocked;
#if NS_DG_TWEENING
        protected Sequence _colorOverlaySequence;
#endif

        private Stack<string> m_Screens = new Stack<string>();

        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake() { }

        // SCREEN MANAGEMENT METHODS

        public void OpenScreen(string sceneName)
        {
            if (m_Screens.Count > 0)
            {
                G.app.GetSceneController(m_Screens.Peek()).enabled = false;
            }
            m_Screens.Push(sceneName);
            G.app.LoadScene(sceneName, false);
        }

        public void CloseScreen(string sceneName)
        {
            if (!m_Screens.Contains(sceneName))
            {
                G.U.Err("Can't find the {0} screen.", sceneName);
                return;
            }
            while (m_Screens.Count > 0)
            {
                // pop all screens stacked on the specified sceneName
                // and stop once the specified sceneName has been popped
                string sn = m_Screens.Pop();
                G.app.UnloadScene(sn);
                if (sn == sceneName) break;
            }
            if (m_Screens.Count > 0)
            {
                G.app.GetSceneController(m_Screens.Peek()).enabled = true;
            }
        }

        // COLOR OVERLAY METHODS

#if NS_DG_TWEENING
        /// <summary>
        /// Fades a color overlay from color 1 to color 2.
        /// </summary>
        /// <returns>The Sequence object used for the fade.</returns>
        /// <param name="color1">Color to fade out/from.</param>
        /// <param name="color2">Color to fade in/to.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="lockDuringFade">If true, no further color overlay methods will work until the duration has elapsed.</param>
        public virtual Sequence FadeColorOverlay(Color color1, Color color2, float duration, bool lockDuringFade = false)
        {
            if (_colorOverlayIsLocked) return DOTween.Sequence().AppendInterval(duration);
            var i = ReplaceColorOverlay(color1);
            _colorOverlayIsLocked = lockDuringFade;
            _colorOverlaySequence = DOTween.Sequence().Append(i.DOColor(color2, duration));
            if (lockDuringFade) _colorOverlaySequence.AppendCallback(() => _colorOverlayIsLocked = false);
            return _colorOverlaySequence;
        }

        /// <summary>
        /// Fades from a color (fades out a color overlay).
        /// </summary>
        /// <returns>The Sequence object used for the fade.</returns>
        /// <param name="color">Color to fade out/from.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="lockDuringFade">If true, no further color overlay methods will work until the duration has elapsed.</param>
        public virtual Sequence FadeFromColor(Color color, float duration, bool lockDuringFade = false)
        {
            Color clear = color.SetAlpha(0); // transparent clone of color
            return FadeColorOverlay(color, clear, duration, lockDuringFade);
        }

        /// <summary>
        /// Fades to a color (fades in a color overlay).
        /// </summary>
        /// <returns>The Sequence object used for the fade.</returns>
        /// <param name="color">Color to fade in/to.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="lockDuringFade">If true, no further color overlay methods will work until the duration has elapsed.</param>
        public virtual Sequence FadeToColor(Color color, float duration, bool lockDuringFade = false)
        {
            Color clear = color.SetAlpha(0); // transparent clone of color
            return FadeColorOverlay(clear, color, duration, lockDuringFade);
        }
#endif

        /// <summary>
        /// Replaces the color overlay on the active scene,
        /// or simply adds an overlay if one does not yet exist.
        /// This function creates both a high sort order Canvas, and a child Image.
        /// </summary>
        /// <returns>The newly created Image component used to display the provided color.</returns>
        /// <param name="color">Color.</param>
        public virtual Image ReplaceColorOverlay(Color color)
        {
            if (_colorOverlayIsLocked) return null;

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

        /// <summary>
        /// Removes the color overlay from the canvas.
        /// </summary>
        public virtual void RemoveColorOverlay()
        {
            if (_colorOverlayIsLocked) return;
#if NS_DG_TWEENING
            if (_colorOverlaySequence != null)
            {
                _colorOverlaySequence.Complete(true);
                _colorOverlaySequence = null;
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