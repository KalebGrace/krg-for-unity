using UnityEngine;

namespace KRG
{
    public abstract class SceneController : MonoBehaviour
    {
        [SerializeField]
        GameObject[] _redundantObjects = default;
        //TODO: convert to separate script (e.g. RedundantSceneObject) subscribing to this awake event, then remove

        /// <summary>
        /// The name of this scene, taken from the SceneName class.
        /// </summary>
        public abstract string sceneName { get; }

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake()
        {
            G.app.AddSceneController(this);
            if (!G.app.isInSingleSceneEditor && _redundantObjects != null)
            {
                for (int i = 0; i < _redundantObjects.Length; i++)
                {
                    Destroy(_redundantObjects[i]);
                }
            }
        }

        /// <summary>
        /// Raises the scene active event.
        /// NOTE: G.app.GoToNextState() will be locked this frame.
        /// </summary>
        public abstract void OnSceneActive();
    }
}
