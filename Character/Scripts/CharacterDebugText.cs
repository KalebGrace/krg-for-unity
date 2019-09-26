using TMPro;
using UnityEngine;

namespace KRG
{
    public class CharacterDebugText : MonoBehaviour
    {
        ICharacterDebugText _characterInterface;
        TextMeshPro _text;

        void Awake()
        {
            _text = this.Require<TextMeshPro>();
        }

        public void Init(MonoBehaviour monoBehaviour)
        {
            _characterInterface = monoBehaviour as ICharacterDebugText;
            if (_characterInterface == null)
            {
                G.U.Warn("This MonoBehaviour must implement the ICharacterDebugText interface to show debug info.",
                    this, monoBehaviour);
                gameObject.Dispose();
            }
        }

        void LateUpdate()
        {
            _text.text = _characterInterface.lateUpdateText;
        }
    }
}
