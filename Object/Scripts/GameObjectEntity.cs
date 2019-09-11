using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class GameObjectEntity : MonoBehaviour
    {
        [SerializeField]
        private GameObjectType m_Type = default;

		[Enum(typeof(CharacterIdentifier))]
		public int CharacterId;

		public SpriteLogic SpriteLogic;

		public GameObjectData Data;
    }
}
