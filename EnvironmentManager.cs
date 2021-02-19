using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _0G.Legacy
{
    public class EnvironmentManager : Manager, ISave, IOnDestroy
    {
        public override float priority => 500;

        // FIELDS

        private readonly List<TeleporterSaveData> m_Teleporters = new List<TeleporterSaveData>();

        // PROPERTIES

        public int CurrentEnvironmentID => G.app.GameplaySceneId;
        public ReadOnlyCollection<TeleporterSaveData> Teleporters => m_Teleporters.AsReadOnly();

        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
            G.save.Subscribe(this);
        }

        public void OnDestroy()
        {
            G.save.Unsubscribe(this);
        }

        // ISAVE METHODS

        public virtual void OnSaving(SaveContext context, ref KRG.SaveFile sf)
        {
            if (context != SaveContext.SaveFile) return;

            sf.teleporters = m_Teleporters.ToArray();
        }

        public virtual void OnLoading(SaveContext context, KRG.SaveFile sf)
        {
            if (context != SaveContext.SaveFile) return;

            m_Teleporters.Clear();
            if (sf.teleporters != null)
            {
                for (int i = 0; i < sf.teleporters.Length; ++i)
                {
                    m_Teleporters.Add(sf.teleporters[i]);
                }
            }
        }

        // PUBLIC METHODS

        public void ActivateTeleporter(int environmentID, int checkpointID)
        {
            for (int i = 0; i < m_Teleporters.Count; ++i)
            {
                var t = m_Teleporters[i];
                if (t.gameplaySceneId == environmentID && t.checkpointId == checkpointID)
                {
                    t.activated = true;
                    m_Teleporters[i] = t;
                    return;
                }
            }
            var data = new TeleporterSaveData(environmentID, checkpointID, true);
            m_Teleporters.Add(data);
        }
    }
}