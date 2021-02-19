using TMPro;
using UnityEngine;

namespace _0G.Legacy
{
    public class CharacterDebugText : MonoBehaviour
    {
        private ICharacterDebugText m_Interface;
        private TextMeshPro m_Text;

        private void Awake()
        {
            m_Text = this.Require<TextMeshPro>();
        }

        private void LateUpdate()
        {
            m_Text.text = m_Interface.Text;
        }

        public void Init(MonoBehaviour monoBehaviour)
        {
            m_Interface = monoBehaviour as ICharacterDebugText;

            if (m_Interface == null)
            {
                string message = "This MonoBehaviour must implement the" +
                    "ICharacterDebugText interface to show debug info.";

                G.U.Warn(message, this, monoBehaviour);

                gameObject.Dispose();
            }
        }
    }
}