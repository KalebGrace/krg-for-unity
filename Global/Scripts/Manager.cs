using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG {

    public abstract class Manager : IManager {

#region shortcut properties

        protected KRGConfig config { get { return G.config; } }

        protected GameObject gameObject { get { return G.instance.gameObject; } }

        protected MonoBehaviour monoBehaviour { get { return G.instance; } }

        protected Transform transform { get { return G.instance.transform; } }

#endregion

    }
}
