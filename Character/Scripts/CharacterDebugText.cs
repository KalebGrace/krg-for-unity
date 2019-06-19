using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if KRG_X_TMPRO || NS_TMPRO_PAID || NS_TMPRO
using TMPro;
#endif

namespace KRG {

    public class CharacterDebugText : MonoBehaviour {

        ICharacterDebugText _characterInterface;

#if KRG_X_TMPRO || NS_TMPRO_PAID || NS_TMPRO
        TextMeshPro _text;
#else
        TextMesh _text;
#endif

        void Awake() {
#if KRG_X_TMPRO || NS_TMPRO_PAID || NS_TMPRO
            _text = this.Require<TextMeshPro>();
#else
            _text = this.Require<TextMesh>();
#endif
        }

        public void Init(Character character) {
            _characterInterface = character as ICharacterDebugText;
            if (_characterInterface == null) {
                G.U.Warning("This character must implement the ICharacterDebugText interface to show debug info.",
                    this, character);
                gameObject.Dispose();
            }
        }

        void LateUpdate() {
            _text.text = _characterInterface.lateUpdateText;
        }
    }
}
