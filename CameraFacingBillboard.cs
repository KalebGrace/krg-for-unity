using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    /// <summary>
    /// Camera-facing billboard.
    /// Add this script to any GameObject to ensure it always faces a particular camera.
    /// Handlers must be added to rotYGetter and (optionally) posGetter for this to work.
    /// </summary>
    public class CameraFacingBillboard : MonoBehaviour, IBodyComponent
    {
        #region Static

        static Vector3 _pos;
        static float _rotY;

        /// <summary>
        /// Gets the pos (position) values.
        /// </summary>
        /// <value>The pos (position) values.</value>
        public static Vector3 pos
        {
            get
            {
                if (posGetter != null)
                {
                    if (posGetter.Target == null || !posGetter.Target.Equals(null))
                    {
                        //This is either a static method OR an instance method with a valid target.
                        _pos = posGetter();
                    }
                    else
                    {
                        //This is an instance method with an invalid target, so remove it.
                        posGetter = null;
                    }
                }
                return _pos;
            }
        }

        /// <summary>
        /// Gets or sets the pos getter.
        /// This "getter" can be used to return new pos (position) values.
        /// Change it to handle e.g. player character position in relation to this.
        /// </summary>
        /// <value>The pos getter.</value>
        public static System.Func<Vector3> posGetter { get; set; }

        /// <summary>
        /// Gets the rotY (Y rotation) value.
        /// </summary>
        /// <value>The rotY (Y rotation) value.</value>
        public static float rotY
        {
            get
            {
                if (rotYGetter != null)
                {
                    if (rotYGetter.Target == null || !rotYGetter.Target.Equals(null))
                    {
                        //This is either a static method OR an instance method with a valid target.
                        _rotY = rotYGetter();
                    }
                    else
                    {
                        //This is an instance method with an invalid target, so remove it.
                        rotYGetter = null;
                    }
                }
                return _rotY;
            }
        }

        /// <summary>
        /// Gets or sets the rotY getter.
        /// This "getter" can be used to return a new rotY (Y rotation) value.
        /// Change it to handle e.g. field camera rotation or first-person lookaround.
        /// </summary>
        /// <value>The rotY getter.</value>
        public static System.Func<float> rotYGetter { get; set; }

        #endregion

        [SerializeField]
        [FormerlySerializedAs("m_useInitialRotation")]
        bool _useInitialRotation = true;

        [SerializeField]
        private GameObjectBody m_Body = default;

        float _initialRotY;

        public bool useInitialRotation
        {
            get
            {
                return _useInitialRotation;
            }
            set
            {
                _useInitialRotation = value;
            }
        }

        public GameObjectBody Body => m_Body;

        public void InitBody(GameObjectBody body)
        {
            m_Body = body;
        }

        void Start()
        {
            _initialRotY = transform.eulerAngles.y;
        }

        void LateUpdate()
        {
            UpdateFacing();
            //UpdateFlipX();
        }

        void UpdateFacing()
        {
            float offsetRotY = _useInitialRotation ? _initialRotY : 0;
            transform.eulerAngles = transform.eulerAngles.SetY((rotY + offsetRotY) % 360);
        }

        /*
        void UpdateFlipX() {
            Vector3 myPos = transform.position;
            float myRotY = transform.eulerAngles.y;
            float ry = (rotY - myRotY).Rotation().clamped_0_360x;
            if (ry < 45 || ry >= 315) {
                //ry ≈ 0
                isFlippedX = false;
            } else if (ry >= 135 && ry < 225) {
                //ry ≈ 180
                isFlippedX = true;
            } else {
                //get the *direction* from (PC) position "pos" to my position "myPos"
                Vector3 dir = myPos - pos;
                //rotate this so myPos is equally likely on either side of the x-axis
                dir = Quaternion.Euler(0, -rotY, 0) * dir;
                //flip as needed
                if (ry >= 45 && ry < 135) {
                    //flip true when ry≈ 90 and (PC)pos left of myPos
                    isFlippedX = dir.x >= 0;
                } else {
                    //flip true when ry≈270 and myPos left of (PC)pos
                    isFlippedX = dir.x < 0;
                }
            }
        }
        */
    }
}