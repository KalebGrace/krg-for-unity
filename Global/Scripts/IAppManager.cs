using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public interface IAppManager {

#region properties

        /// <summary>
        /// Gets a value indicating whether this <see cref="KRG.IAppManager"/>
        /// is in the Unity Editor while running a single scene (and the App State is None).
        /// </summary>
        /// <value><c>true</c> if is in single scene editor; otherwise, <c>false</c>.</value>
        bool isInSingleSceneEditor { get; }

        bool isQuitting { get; }

        string masterSceneName { get; }

#endregion

#region methods

        void AddSceneActivationListener(string sceneName, System.Action listener);

        void AddSceneController(SceneController sceneController);

        void RemoveSceneActivationListener(string sceneName, System.Action listener);

        void Quit();

#endregion

    }
}
