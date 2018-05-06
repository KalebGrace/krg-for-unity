using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    /// <summary>
    /// Camera-facing billboard.
    /// Add this script to any GameObject to ensure it always faces a particular camera.
    /// Handlers must be added to rotYGetter and (optionally) posGetter for this to work.
    /// </summary>
    public class CameraFacingBillboard : MonoBehaviour {

#region Static

        static Vector3 _pos;
        static float _rotY;

        /// <summary>
        /// Gets the pos (position) values.
        /// </summary>
        /// <value>The pos (position) values.</value>
        public static Vector3 pos {
            get {
                if (posGetter != null) {
                    if (posGetter.Target == null || !posGetter.Target.Equals(null)) {
                        //This is either a static method OR an instance method with a valid target.
                        _pos = posGetter();
                    } else {
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
        public static float rotY {
            get {
                if (rotYGetter != null) {
                    if (rotYGetter.Target == null || !rotYGetter.Target.Equals(null)) {
                        //This is either a static method OR an instance method with a valid target.
                        _rotY = rotYGetter();
                    } else {
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

        //values listed below are simply DEFAULTS; check inspector for actual values
        [SerializeField]
        [FormerlySerializedAs("m_flipModeX")]
        FlipMode _flipModeX;
        [SerializeField]
        [FormerlySerializedAs("m_graphicsController")]
        GraphicsController _graphicsController;
        [SerializeField]
        [FormerlySerializedAs("m_useInitialRotation")]
        bool _useInitialRotation = true;

        Transform _graphicsControllerTransform;
        float _initialRotY;
        Transform _transform;

#region _isFlippedX Scope

        bool _isFlippedX;

        //_isFlippedX should only be referenced by the isFlippedX property

        public bool isFlippedX {
            get {
                return _isFlippedX;
            }
            private set {
                if (_isFlippedX != value) {
                    _isFlippedX = value;
                    switch (_flipModeX) {
                        case FlipMode.FlipHere:
                            _transform.localScale = _transform.localScale.Multiply(x: -1);
                            break;
                        case FlipMode.FlipOnGraphicsController:
                            G.U.Require(_graphicsController, "GraphicsController");
                            _graphicsController.FlipX();
                            break;
                        default:
                            G.U.Unsupported(this, _flipModeX);
                            break;
                    }
                }
            }
        }

#endregion

        public FlipMode flipModeX {
            get {
                return _flipModeX;
            }
            set {
                if (_flipModeX != FlipMode.None && _flipModeX != value) {
                    isFlippedX = false;
                }
                _flipModeX = value;
            }
        }

        public GraphicsController graphicsController {
            get {
                return _graphicsController;
            }
            set {
                _graphicsController = value;
                _graphicsControllerTransform = value != null ? value.transform : null;
            }
        }

        public bool useInitialRotation {
            get {
                return _useInitialRotation;
            }
            set {
                _useInitialRotation = value;
            }
        }

        void Start() {
            _transform = transform;
            _initialRotY = _transform.eulerAngles.y;
        }

        void LateUpdate() {
            UpdateFacing();
            UpdateFlipX();
        }

        void UpdateFacing() {
            float offsetRotY = _useInitialRotation ? _initialRotY : 0;
            _transform.eulerAngles = _transform.eulerAngles.SetY((rotY + offsetRotY) % 360);
        }

        void UpdateFlipX() {
            Vector3 myPos = Vector3.zero;
            float myRotY = 0;
            switch (_flipModeX) {
                case FlipMode.None:
                    //do nothing
                    return;
                case FlipMode.FlipHere:
                    //we will assume that flipping here will want to use this transform
                    myPos = _transform.position;
                    myRotY = _transform.eulerAngles.y;
                    break;
                case FlipMode.FlipOnGraphicsController:
                    //we will assume that flipping on GC will want to use the GC's transform
                    G.U.Require(_graphicsControllerTransform, "GraphicsController.transform");
                    myPos = _graphicsControllerTransform.position;
                    myRotY = _graphicsControllerTransform.eulerAngles.y;
                    break;
            }
            float ry = (rotY - myRotY).ClampRotationDegrees();
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
    }
}
