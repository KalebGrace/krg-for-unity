using UnityEngine;

namespace KRG
{
    [ExecuteAlways]
    public sealed class GameObjectBody : MonoBehaviour
    {
        // SERIALIZED FIELDS

        public GameObjectType GameObjectType = default;

        [Enum(typeof(CharacterID))]
        public int CharacterID = default;

        public GameObjectRefs Refs = default;

        public GameObjectData Data = default;

        // CHARACTER PROPERTIES

        public CharacterDossier CharacterDossier { get; set; }

        public CharacterType CharacterType => CharacterDossier?.CharacterType ?? CharacterType.None;

        public bool IsCharacter => GameObjectType == GameObjectType.Character;
        public bool IsCharacterError => IsCharacter && CharacterDossier == null;
        public bool IsPlayerCharacter => CharacterType.IsPlayerCharacter();
        public bool IsBoss => CharacterType.IsBoss();
        public bool IsEnemy => CharacterType.IsEnemy();
        public bool IsEnemyOrBoss => CharacterType.IsEnemyOrBoss();
        public bool IsNPC => CharacterType.IsNPC();

        // GRAPHIC PROPERTIES

        public bool IsFlippedX => Refs.GraphicController?.IsFlippedX ?? false;

        // MONOBEHAVIOUR METHODS

        private void OnValidate()
        {
            Refs.AutoAssign(this);
        }

        private void Awake()
        {
            if (G.obj.Register(this))
            {
                switch (GameObjectType)
                {
                    case GameObjectType.None:
                        break;
                    case GameObjectType.Character:
                        InitCharacter();
                        break;
                    case GameObjectType.Attack:
                        InitAttack();
                        break;
                    default:
                        G.U.Err("Unsupported GameObjectType {0}.", GameObjectType);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            G.obj.Deregister(this);
        }

        // BODY METHODS

        public void Dispose()
        {
            // TODO: object pooling logic goes here, eventually

            gameObject.Dispose(); // destroys the entire body, thus calling OnDestroy()
        }

        private void InitCharacter()
        {
            if (IsCharacterError)
            {
                G.U.Err("Character error for character ID {0}.", CharacterID);
                return;
            }

            if (G.U.IsEditMode(this))
            {
                // TODO: should auto-assign these values, eventually

                string properTag = CharacterDossier.CharacterType.ToTag();
                if (!gameObject.CompareTag(properTag))
                {
                    G.U.Err("Invalid tag {0}. Should be {1}.", gameObject.tag, properTag);
                }

                if (IsPlayerCharacter && gameObject.layer != Layer.PCBoundBox)
                {
                    G.U.Err("Invalid layer {0}. Should be {1}.", gameObject.layer, Layer.PCBoundBox);
                }
            }

#if DEBUG_VISIBILITY
            G.U.Todo("revise this");
            /*
            Transform center = VisRect.transform;
            Instantiate(G.config.characterDebugTextPrefab, center).Init(this);
            if (Refs.GraphicController != null)
            {
                Refs.GraphicController.RasterAnimationInfo = Instantiate(G.config.rasterAnimationInfoPrefab, center);
            }
            */
#endif
        }

        private void InitAttack()
        {

        }
    }
}
