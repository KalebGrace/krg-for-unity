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

        public void Init(Character character)
        {
            _characterInterface = character as ICharacterDebugText;
            if (_characterInterface == null)
            {
                G.U.Warning("This character must implement the ICharacterDebugText interface to show debug info.",
                    this, character);
                gameObject.Dispose();
            }
        }

        void LateUpdate()
        {
            _text.text = _characterInterface.lateUpdateText;
        }
    }
}
