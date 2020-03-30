using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class SaveManager : Manager
    {
        public override float priority => 5;

        // SAVE FILE

        private SaveFile m_CurrentCheckpoint;

        private readonly object m_SaveLock = new object();

        public delegate void SaveFileWriteHandler(ref SaveFile sf);
        public delegate void SaveFileReadHandler(SaveFile sf);

        private event SaveFileWriteHandler Saving;
        private event SaveFileReadHandler SavingCompleted;
        private event SaveFileReadHandler Loading;
        private event SaveFileReadHandler LoadingCompleted;

        protected virtual string DefaultGameplaySaveKey => SaveFile.GetKeyPrefix(SaveContext.ContinueCheckpoint) + 0;

        public virtual bool HasDefaultGameplaySave
        {
            get
            {
#if KRG_X_EASY_SAVE_3
                return ES3.KeyExists(DefaultGameplaySaveKey);
#else
                return false;
#endif
            }
        }

        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
#if KRG_X_EASY_SAVE_3
            try
            {
                m_CurrentCheckpoint = ES3.Load(DefaultGameplaySaveKey, SaveFile.New(SaveContext.ContinueCheckpoint));
            }
            catch (System.Exception ex)
            {
                G.U.Err("Unable to load continue checkpoint data from save file.", DefaultGameplaySaveKey, ex);
                m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);
            }
#endif
            m_CurrentCheckpoint.Validate();
        }

        // PUBLIC METHODS

        public void Subscribe(ISave iSave)
        {
            Saving += iSave.OnSaving;
            Loading += iSave.OnLoading;

            if (iSave is ISaveComplete iSaveComplete)
            {
                SavingCompleted += iSaveComplete.OnSavingCompleted;
                LoadingCompleted += iSaveComplete.OnLoadingCompleted;
            }
        }

        public void Unsubscribe(ISave iSave)
        {
            Saving -= iSave.OnSaving;
            Loading -= iSave.OnLoading;

            if (iSave is ISaveComplete iSaveComplete)
            {
                SavingCompleted -= iSaveComplete.OnSavingCompleted;
                LoadingCompleted -= iSaveComplete.OnLoadingCompleted;
            }
        }

        public Vector3 GetCurrentCheckpointPosition()
        {
            return m_CurrentCheckpoint.position;
        }

        public bool IsCurrentCheckpoint(AlphaBravo checkpointName)
        {
            return (int)checkpointName == m_CurrentCheckpoint.checkpointId &&
                 G.app.GameplaySceneId == m_CurrentCheckpoint.gameplaySceneId;
        }

        public void SaveCheckpoint(AlphaBravo checkpointName = 0)
        {
            lock (m_SaveLock)
            {
                m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);

                m_CurrentCheckpoint.checkpointId = (int)checkpointName;

                m_CurrentCheckpoint.switchStates = m_SwitchStates;

                Saving?.Invoke(ref m_CurrentCheckpoint);

                WriteToDisk(m_CurrentCheckpoint);

                SavingCompleted?.Invoke(m_CurrentCheckpoint);
            }
        }

        protected virtual void WriteToDisk(SaveFile sf)
        {
#if KRG_X_EASY_SAVE_3
            ES3.Save<SaveFile>(sf.Key, sf);
#endif
        }

        public void LoadCheckpoint()
        {
            Load(m_CurrentCheckpoint);
        }

        private void Load(SaveFile sf)
        {
            lock (m_SaveLock)
            {
                //TODO: add this for implementing quicksaves/hardsaves
                //ReadFromDisk();

                m_SwitchStates = sf.switchStates;

                Loading?.Invoke(sf);

                LoadingCompleted?.Invoke(sf);
            }
        }

        public virtual void EraseDefaultGameplaySave()
        {
            ES3.DeleteKey(DefaultGameplaySaveKey);
            G.app.ResetGameplaySceneId(); //TODO: fix this
            m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);
        }

        // SWITCH STATES

        private Dictionary<int, int> m_SwitchStates = new Dictionary<int, int>();

        public bool GetSwitchState(Switch @switch, out int stateIndex)
        {
            if (m_SwitchStates.ContainsKey(@switch.ID))
            {
                stateIndex = m_SwitchStates[@switch.ID];
                return true;
            }
            stateIndex = -1;
            return false;
        }

        public void SetSwitchState(Switch @switch)
        {
            if (m_SwitchStates.ContainsKey(@switch.ID))
            {
                m_SwitchStates[@switch.ID] = @switch.StateIndex;
            }
            else
            {
                m_SwitchStates.Add(@switch.ID, @switch.StateIndex);
            }
        }
    }
}
