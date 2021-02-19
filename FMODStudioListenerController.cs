using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_FMOD
using FMODUnity;
#endif

namespace _0G.Legacy
{
    public class FMODStudioListenerController : MonoBehaviour
    {
#if NS_FMOD

        static readonly List<FMODStudioListenerController> _controllers = new List<FMODStudioListenerController>();

        static FMODStudioListenerController _activeController;

        StudioListener _fmodListener;

        AudioListener _unityListener;

        void Awake()
        {
            _controllers.Add(this);
            if (_activeController != null)
            {
                DeactivateActiveController();
            }
            Activate();
        }

        void OnDestroy()
        {
            _controllers.Remove(this);
            if (_activeController == this)
            {
                DeactivateActiveController();
                ReactivateRemainingController();
            }
        }

        void Activate()
        {
            _activeController = this;
            GuaranteeListeners();
        }

        void GuaranteeListeners()
        {
            G.U.Assert(_fmodListener == null);
            _fmodListener = G.U.Guarantee<StudioListener>(this);

            G.U.Assert(_unityListener == null);
            _unityListener = G.U.Guarantee<AudioListener>(this);
        }

        void RemoveListeners()
        {
            if (_fmodListener != null)
            {
                Destroy(_fmodListener);
                _fmodListener = null;
            }
            if (_unityListener != null)
            {
                Destroy(_unityListener);
                _unityListener = null;
            }
        }

        static void DeactivateActiveController()
        {
            _activeController.RemoveListeners();
            _activeController = null;
        }

        static void ReactivateRemainingController()
        {
            if (_controllers.Count > 0)
            {
                _controllers[0].Activate();
            }
        }

#endif
    }
}