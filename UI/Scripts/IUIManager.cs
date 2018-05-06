using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    public interface IUIManager {

#region methods

        /// <summary>
        /// Adds a color overlay to the active scene. This creates both a high sort order Canvas, and a child Image.
        /// </summary>
        /// <returns>The newly created Image component used to display the provided color.</returns>
        /// <param name="color">Color.</param>
        Image AddColorOverlay(Color color);

#if NS_DG_TWEENING

        /// <summary>
        /// Fades a color overlay from color 1 to color 2.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color1">Color 1.</param>
        /// <param name="color2">Color 2.</param>
        /// <param name="duration">Duration in seconds.</param>
        Tweener FadeColorOverlay(Color color1, Color color2, float duration);

        /// <summary>
        /// Fades in a color overlay.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color">Color.</param>
        /// <param name="duration">Duration in seconds.</param>
        Tweener FadeInColorOverlay(Color color, float duration);

        /// <summary>
        /// Fades out a color overlay.
        /// </summary>
        /// <returns>The Tweener object used for the fade.</returns>
        /// <param name="color">Color.</param>
        /// <param name="duration">Duration in seconds.</param>
        Tweener FadeOutColorOverlay(Color color, float duration);

#endif

        /// <summary>
        /// Removes the color overlay from the canvas.
        /// </summary>
        void RemoveColorOverlay();

#endregion

    }
}
