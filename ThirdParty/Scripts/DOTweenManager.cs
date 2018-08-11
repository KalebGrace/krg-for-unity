using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NS_DG_TWEENING
using DG.Tweening;
#endif

namespace KRG {

    /// <summary>
    /// SCRIPT EXECUTION ORDER: #03
    /// </summary>
    public class DOTweenManager : Manager, IDOTweenManager {

#region IManager implementation

        public override void Awake() {
#if NS_DG_TWEENING
            if (config.doTweenUseInitSettings) {
                DOTween.Init(config.doTweenRecycleAllByDefault, config.doTweenUseSafeMode, config.doTweenLogBehaviour);
            } else {
                DOTween.Init();
            }
#endif
        }

#endregion

    }
}
