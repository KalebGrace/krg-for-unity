using System.Collections.Generic;
using UnityEngine;

namespace KRG
{
    public class SaveManager : Manager
    {
        public override float priority => 5;

        // SAVE FILE

        private int m_SaveSlotIndex;
        private SaveFile m_SaveFile;

        private readonly object m_SaveLock = new object();

        public delegate void SaveFileWriteHandler(ref SaveFile sf);
        public delegate void SaveFileReadHandler(SaveFile sf);

        private event SaveFileWriteHandler Saving;
        private event SaveFileReadHandler SavingCompleted;
        private event SaveFileReadHandler Loading;
        private event SaveFileReadHandler LoadingCompleted;

        public virtual int SaveSlotCount => 3;

        protected virtual string ES3Key => "SaveFile";

        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
            SetSaveSlot(1);

            if (!G.save.SaveFileExists())
            {
                G.save.CreateSaveFile();
            }
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

        public virtual void SetSaveSlot(int saveSlotIndex)
        {
            m_SaveSlotIndex = saveSlotIndex;
            m_SaveFile = default;
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                if (!SaveFileExists(saveSlotIndex)) return;
                m_SaveFile = ES3.Load<SaveFile>(ES3Key, filePath);
                m_SaveFile.Validate();
            }
            catch (System.Exception ex)
            {
                G.U.Err("Error while loading save file.", ES3Key, filePath, ex);
            }
#endif
        }

        public virtual bool SaveFileExists(int saveSlotIndex = 0)
        {
            bool exists = false;
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                exists = ES3.FileExists(filePath) && ES3.KeyExists(ES3Key, filePath);
            }
            catch (System.Exception ex)
            {
                G.U.Err("Error while checking save file.", ES3Key, filePath, ex);
            }
#endif
            return exists;
        }

        public virtual void CreateSaveFile()
        {
            G.app.ResetGameplaySceneId(); //TODO: fix this
            m_SaveFile = SaveFile.New();
        }

        public virtual void DeleteSaveFile(int saveSlotIndex = 0)
        {
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                ES3.DeleteFile(filePath);
            }
            catch (System.Exception ex)
            {
                G.U.Err("Error while deleting save file.", ES3Key, filePath, ex);
            }
#endif
        }

        public virtual void DeleteAllSaveFiles()
        {
            for (int i = 1; i <= SaveSlotCount; ++i)
            {
                DeleteSaveFile(i);
            }
        }

        public Vector3 GetCurrentCheckpointPosition()
        {
            return m_SaveFile.position;
        }

        public bool IsCurrentCheckpoint(AlphaBravo checkpointName)
        {
            return (int) checkpointName == m_SaveFile.checkpointId &&
                G.app.GameplaySceneId == m_SaveFile.gameplaySceneId;
        }

        public void SaveCheckpoint(AlphaBravo checkpointName = 0)
        {
            lock(m_SaveLock)
            {
                m_SaveFile = SaveFile.New();

                m_SaveFile.checkpointId = (int)checkpointName;

                m_SaveFile.switchStates = m_SwitchStates;

                Saving?.Invoke(ref m_SaveFile);

                WriteToDisk(m_SaveFile);

                SavingCompleted?.Invoke(m_SaveFile);
            }
        }

        protected virtual void WriteToDisk(SaveFile sf)
        {
#if KRG_X_EASY_SAVE_3
            ES3.Save<SaveFile>(ES3Key, sf, GetES3FilePath());
#endif
        }

        public void LoadCheckpoint()
        {
            Load(m_SaveFile);
        }

        private void Load(SaveFile sf)
        {
            lock(m_SaveLock)
            {
                //TODO: add this for implementing quicksaves/hardsaves
                //ReadFromDisk();

                m_SwitchStates = sf.switchStates;

                Loading?.Invoke(sf);

                LoadingCompleted?.Invoke(sf);
            }
        }

        // PROTECTED METHODS

        protected virtual string GetES3FilePath(int saveSlotIndex = 0)
        {
            if (saveSlotIndex == 0)
            {
                saveSlotIndex = m_SaveSlotIndex;
            }
            return string.Format("{0}{1:00}.es3", ES3Key, saveSlotIndex);
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