using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    [CreateAssetMenu(
        fileName = "SomeOne_CharacterDossier.asset",
        menuName = "KRG Scriptable Object/CharacterDossier",
        order = 304
    )]
    public sealed class CharacterDossier : ScriptableObject
    {
        [Enum(typeof(CharacterIdentifier))]
        public int CharacterId;

        public string Name;

        public CharacterType Type;

        public CharacterData Data;
    }
}
