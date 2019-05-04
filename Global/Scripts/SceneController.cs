using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public abstract class SceneController : MonoBehaviour {

#region serialized fields

        [SerializeField]
        GameObject[] _redundantObjects = default;

#endregion

#region properties

        /// <summary>
        /// The name of this scene, taken from the SceneName class.
        /// </summary>
        public abstract string sceneName { get; }

#endregion

#region methods

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake() {
            G.app.AddSceneController(this);
            if (!G.app.isInSingleSceneEditor && _redundantObjects != null) {
                for (int i = 0; i < _redundantObjects.Length; i++) {
                    Destroy(_redundantObjects[i]);
                }
            }
        }

        /// <summary>
        /// Raises the scene active event.
        /// NOTE: G.app.GoToNextState() will be locked this frame.
        /// </summary>
        public abstract void OnSceneActive();

#endregion

    }
}
