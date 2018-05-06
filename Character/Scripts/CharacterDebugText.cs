using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_TMPRO
using TMPro;
#endif

namespace KRG {

    public class CharacterDebugText : MonoBehaviour {

#if NS_TMPRO

        ICharacterDebugText _characterInterface;
        TextMeshPro _text;

        void Awake() {
            _text = G.U.Require<TextMeshPro>(this);
        }

        public void Init(Character character) {
            _characterInterface = character as ICharacterDebugText;
            if (_characterInterface == null) {
                G.U.Warning("This character must implement the ICharacterDebugText interface to show debug info.",
                    this, character);
                G.End(gameObject);
            }
        }

        void LateUpdate() {
            _text.text = _characterInterface.lateUpdateText;
        }

#endif

    }
}
