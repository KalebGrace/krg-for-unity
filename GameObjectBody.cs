using System;
using UnityEngine;

namespace _0G.Legacy
{
    [ExecuteAlways]
    public sealed class GameObjectBody : MonoBehaviour, ICharacterDebugText
    {
        // SERIALIZED FIELDS

        public GameObjectType GameObjectType = default;

        [Enum(typeof(CharacterID))]
        public int CharacterID = default;

        [SerializeField, Enum(typeof(FacingDirection))]
        private int m_FacingDirection = default;
        [SerializeField, HideInInspector]
        private int m_FacingDirectionOrig = default; // only used OnValidate

        [SerializeField, Enum(typeof(TimeThreadInstance))]
        private int m_TimeThread = (int) TimeThreadInstance.UseDefault;

        public GameObjectRefs Refs = default;

        public GameObjectData Data = default;

        // RUNTIME EVENTS & DELEGATES

        public event FacingDirectionHandler FacingDirectionChanged;

        public delegate void FacingDirectionHandler(Direction oldDirection, Direction newDirection);

        // TIME PROPERTIES

        public TimeThread TimeThread
        {
            get
            {
                TimeThreadInstance tti = (TimeThreadInstance) m_TimeThread;
                if (tti == TimeThreadInstance.UseDefault)
                {
                    switch (GameObjectType)
                    {
                        case GameObjectType.UI:
                            tti = TimeThreadInstance.Application;
                            break;
                        default:
                            tti = TimeThreadInstance.Field;
                            break;
                    }
                }
                return G.time.GetTimeThread(tti);
            }
        }

        // CHARACTER PROPERTIES

        public CharacterDossier CharacterDossier { get; set; }

        public CharacterType CharacterType => CharacterDossier?.CharacterType ?? CharacterType.None;

        public bool IsCharacter => GameObjectType == GameObjectType.Character;
        public bool IsPlayerCharacter => CharacterType == CharacterType.PlayerCharacter;
        public bool IsNPC => CharacterType == CharacterType.NonPlayerCharacter;
        public bool IsEnemyOrBoss => IsEnemy || IsBoss;
        public bool IsEnemy => CharacterType == CharacterType.Enemy;
        public bool IsBoss => CharacterType == CharacterType.Boss;
        public bool IsGalleryAnimation => GameObjectType == GameObjectType.GalleryAnimation;

        // ATTACK PROPERTIES

        public bool IsAttack => GameObjectType == GameObjectType.Attack;

        // GRAPHICAL & VISUAL PROPERTIES

        public Transform CenterTransform => Refs.VisRect?.transform ?? transform;

        public Direction FacingDirection
        {
            get => (Direction) m_FacingDirection;
            set
            {
                if (m_FacingDirection != (int) value)
                {
                    OnFacingDirectionChange((Direction) m_FacingDirection, value);
                    m_FacingDirection = (int) value;
                }
            }
        }

        // DEBUG PROPERTIES

        string ICharacterDebugText.Text { get; set; }

        // MONOBEHAVIOUR METHODS

        private void OnValidate()
        {
            if (m_FacingDirectionOrig != m_FacingDirection)
            {
                OnFacingDirectionChange((Direction) m_FacingDirectionOrig, (Direction) m_FacingDirection);
                m_FacingDirectionOrig = m_FacingDirection;
            }

            Refs.AutoAssign(this);
        }

        private void Awake()
        {
            if (G.obj.Register(this))
            {
                switch (GameObjectType)
                {
                    case GameObjectType.Character:
                        InitCharacter();
                        break;
                    case GameObjectType.Attack:
                        InitAttack();
                        break;
                    case GameObjectType.Item:
                        InitItem();
                        break;
                    case GameObjectType.VFX:
                        InitVFX();
                        break;
                    case GameObjectType.UI:
                        InitUI();
                        break;
                    case GameObjectType.GalleryAnimation:
                    case GameObjectType.None:
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

            if (GameObjectType == GameObjectType.VFX)
            {
                Refs.GraphicController.StopVFX(() => gameObject.Dispose());
            }
            else
            {
                gameObject.Dispose(); // destroys the entire body, thus calling OnDestroy()
            }
        }

        private void InitCharacter()
        {
            if (IsCharacter && CharacterDossier == null)
            {
                G.U.Err("Character error for character ID {0}.", CharacterID);
                return;
            }

            if (G.U.IsEditMode(this))
            {
                string properTag = CharacterDossier.CharacterType.ToTag();
                if (!gameObject.CompareTag(properTag) && !gameObject.CompareTag(CharacterTag.Animation.ToString()))
                {
                    G.U.Err("Invalid tag {0}. Should be {1}.", gameObject.tag, properTag);
                }

                if (IsPlayerCharacter && gameObject.layer != Layer.PCBoundBox)
                {
                    G.U.Err("Invalid layer {0}. Should be {1}.", gameObject.layer, Layer.PCBoundBox);
                }
            }
            else
            {
                if (G.U.IsDebugMode)
                {
                    Transform centerTransform = Refs.VisRect?.transform ?? transform;
                    Instantiate(G.config.characterDebugTextPrefab, centerTransform).Init(this);
                }
            }
        }

        private void InitAttack() { }

        private void InitItem() { }

        private void InitVFX() { }

        private void InitUI() { }

        private void OnFacingDirectionChange(Direction oldDirection, Direction newDirection)
        {
            if (G.U.IsPlayMode(this))
            {
                FacingDirectionChanged?.Invoke(oldDirection, newDirection);
            }
            else
            {
                // get all IFacingDirection components, including inactive ones, then call methods

                IFacingDirection[] components = GetComponentsInChildren<IFacingDirection>(true);

                foreach (IFacingDirection c in components)
                {
                    if (c.Body == this)
                    {
                        c.OnFacingDirectionChange(oldDirection, newDirection);
                    }
                }
            }
        }
    }
}