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

        public CharacterDossier CharacterDossier { get; private set; }

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
            G.U.Log("GameObjectBody OnValidate for {0}.", name);
            AutoAssignRefs();
        }

        private void Awake()
        {
            G.U.Log("GameObjectBody Awake for {0}.", name);
            if (!IsValid()) return;

            G.obj.Register(this);

            switch (GameObjectType)
            {
                case GameObjectType.None:
                    break;
                case GameObjectType.Character:
                    InitCharacter();
                    break;
                default:
                    G.U.Err("Unsupported GameObjectType {0}.", GameObjectType);
                    break;
            }
        }

        private void OnDestroy()
        {
            G.U.Log("GameObjectBody OnDestroy for {0}.", name);
            G.obj.Deregister(this);
        }

        // BODY METHODS

        public void Dispose()
        {
            gameObject.Dispose();

            // TODO: object pooling logic goes here
        }

        private bool IsValid()
        {
            if (Application.IsPlaying(this))
            {
                if (IsPlayerCharacter && G.config.IsSinglePlayerGame && G.obj.PlayerCharacters.Count > 0)
                {
                    // this is a duplicate player character
                    Dispose();
                    return false;
                }
            }

            return true;
        }

        private void InitCharacter()
        {
            if (CharacterID == 0)
            {
                return;
            }

            CharacterDossier = G.obj.CharacterDossiers[CharacterID];

            if (IsCharacterError)
            {
                G.U.Err("Character error for character ID {0}.", CharacterID);
                return;
            }

            string properTag = CharacterDossier.CharacterType.ToTag();
            if (!gameObject.CompareTag(properTag))
            {
                G.U.Err("Invalid tag {0}. Should be {1}.", gameObject.tag, properTag);
            }

            if (IsPlayerCharacter && gameObject.layer != Layer.PCBoundBox)
            {
                G.U.Err("Invalid layer {0}. Should be {1}.", gameObject.layer, Layer.PCBoundBox);
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

        private void AutoAssignRefs()
        {
            // automatically assign references, but only in edit mode

            if (Application.isPlaying) return;

            // assign Unity game object & component references

            AssignRef(ref Refs.GraphicGameObject);

            AssignRef(ref Refs.Animator);
            AssignRef(ref Refs.Collider);
            AssignRef(ref Refs.Rigidbody);

            // get all IBodyComponents, including inactive ones, then assign references

            IBodyComponent[] components = GetComponentsInChildren<IBodyComponent>(true);

            foreach (IBodyComponent c in components)
            {
                if (c.Body == null)
                {
                    c.InitBody(this);
                }

                AssignRef(c, ref Refs.GraphicController);
                AssignRef(c, ref Refs.VisRect);
                AssignRef(c, ref Refs.Attacker);
                AssignRef(c, ref Refs.DamageTaker);
            }
        }

        private void AssignRef(ref GameObject bodyRef)
        {
            if (bodyRef == default)
            {
                bodyRef = gameObject;
            }
        }

        private void AssignRef<T>(ref T bodyRef) where T : Component
        {
            if (bodyRef == default)
            {
                bodyRef = GetComponent<T>();
            }
        }

        private static void AssignRef<T>(IBodyComponent c, ref T bodyRef) where T : IBodyComponent
        {
            if (bodyRef == default && c is T newRef)
            {
                bodyRef = newRef;
            }
        }
    }
}
