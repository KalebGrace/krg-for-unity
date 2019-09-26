using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class GameplayHudElement : MonoBehaviour
    {
        //private static readonly Dictionary<int, GameplayHudElement> dict = new Dictionary<int, GameplayHudElement>();

        /*
        [SerializeField, Enum(typeof(GameplayHudElementType))]
        private int m_Type;
        */

        [SerializeField]
        private bool m_StartsHidden = default;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            //if (m_Type != 0) dict.Add(m_Type, this);

            canvasGroup = GetComponent<CanvasGroup>();

            if (m_StartsHidden) Hide();
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
        }

        public void Show()
        {
            canvasGroup.alpha = 1;
        }

        /*
        public static GameplayHudElement GetElement(GameplayHudElementType type)
        {
            return dict[(int)type];
        }
        */
    }
}
