using UnityEngine;

namespace _0G.Legacy
{
    public abstract class SceneController : MonoBehaviour
    {
        public System.Action Destroyed;

        [SerializeField, System.Obsolete("Just delete the object.")]
        GameObject[] _redundantObjects = default;
        //TODO: convert to separate script (e.g. RedundantSceneObject) subscribing to this awake event, then remove

        /// <summary>
        /// The name of this scene, taken from the SceneName class.
        /// </summary>
        public abstract string sceneName { get; }

        public virtual SceneType SceneType => SceneType.None;

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake()
        {
            G.app.AddSceneController(this);

            #region obsolete code
#pragma warning disable CS0618
            if (!G.app.isInSingleSceneEditor && _redundantObjects != null)
            {
                for (int i = 0; i < _redundantObjects.Length; i++)
                {
                    Destroy(_redundantObjects[i]);
                }
            }
#pragma warning restore CS0618
            #endregion
        }

        protected virtual void OnDestroy()
        {
            Destroyed?.Invoke();
        }

        /// <summary>
        /// Raises the scene active event.
        /// NOTE: G.app.GoToNextState() will be locked this frame.
        /// </summary>
        public virtual void OnSceneActive() { }
    }
}