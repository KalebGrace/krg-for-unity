using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRG.Unused {

	[System.Obsolete]
	public class KRGLoader {

		//Previously in KRG.G / KRG.KRGLoader:

		/*
        private void AddManagers() {
            List<ManagerConfig> mcList = _config.managerConfigs;
            ManagerConfig managerConfig;
            Type managerType, configType;
            for (int i = 0; i < mcList.Count; i++) {
                managerConfig = mcList[i];
                managerType = managerConfig.managerType;
                configType = managerConfig.GetType();
                typeof(G)
                    .GetMethod("AddManagerReflective")
                    .MakeGenericMethod(managerType, configType)
                    .Invoke(this, new object[]{ managerConfig });
                //NOTE: there can be problems here if the managerType of the
                //managerConfig is not reciprocal with that Manager type's TConfig
            }
        }

        public void AddManagerReflective<TManager, TConfig>(TConfig managerConfig)
            where TManager : ManagerOld<TConfig>
            where TConfig : ManagerConfig {

            TManager manager = gameObject.AddComponent<TManager>();
            manager.SetConfig(managerConfig);
            m_managers.Add(typeof(TManager), manager);
        }
		*/

	}
}
