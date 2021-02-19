using System;
using System.Collections.Generic;
using KRG;
using UnityEngine;

namespace _0G.Legacy
{
    public class SaveManager : Manager
    {
        public override float priority => 5;

        private SaveFile m_SaveFile;
        private float m_TimeLoaded;

        private readonly object m_MetaGameLock = new object();
        private readonly object m_SaveFileLock = new object();

        public delegate void SaveFileWriteHandler(SaveContext context, ref SaveFile sf);
        public delegate void SaveFileReadHandler(SaveContext context, SaveFile sf);

        private event SaveFileWriteHandler Saving;
        private event SaveFileReadHandler SavingCompleted;
        private event SaveFileReadHandler Loading;
        private event SaveFileReadHandler LoadingCompleted;

        public virtual int SaveSlot { get; protected set; }
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

        public virtual void MetaSave()
        {
            lock (m_MetaGameLock)
            {
                SaveFile sf = new SaveFile();
                Saving?.Invoke(SaveContext.MetaGame, ref sf);
                SavingCompleted?.Invoke(SaveContext.MetaGame, sf);
            }
        }

        public virtual void SetSaveSlot(int saveSlotIndex)
        {
            SaveSlot = saveSlotIndex;
            m_SaveFile = default;
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                if (!SaveFileExists(saveSlotIndex)) return;
                m_SaveFile = ES3.Load<SaveFile>(ES3Key, filePath);
                m_SaveFile.Validate();
                m_TimeLoaded = Time.time;
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            m_TimeLoaded = Time.time;
        }

        public virtual void DeleteSaveFile(int saveSlotIndex = 0)
        {
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                ES3.DeleteFile(filePath);
            }
            catch (Exception ex)
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

        public float GetGameplayDuration(int saveSlotIndex = 0)
        {
#if KRG_X_EASY_SAVE_3
            string filePath = GetES3FilePath(saveSlotIndex);
            try
            {
                if (!SaveFileExists(saveSlotIndex)) return -1;
                SaveFile sf = ES3.Load<SaveFile>(ES3Key, filePath);
                sf.Validate();
                return sf.gameplayDuration;
            }
            catch (Exception ex)
            {
                G.U.Err("Error while getting gameplay duration.", ES3Key, filePath, ex);
            }
#endif
            return -1;
        }

        public string GetFormattedGameplayDuration(int saveSlotIndex = 0)
        {
            float gd = GetGameplayDuration(saveSlotIndex);
            if (gd.Ap(-1))
            {
                return "NEW";
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(gd);
                int h = time.Hours + time.Days * 24;
                int m = time.Minutes;
                return string.Format("{0}:{1:00}", h, m);
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
            lock(m_SaveFileLock)
            {
                MetaSave();

                SaveFile oldSF = m_SaveFile;

                m_SaveFile = SaveFile.New();

                float d = Time.time - m_TimeLoaded;

                m_TimeLoaded = Time.time;

                m_SaveFile.gameplayDuration = oldSF.gameplayDuration + d;

                m_SaveFile.checkpointId = (int)checkpointName;

                m_SaveFile.switchStates = m_SwitchStates;

                Saving?.Invoke(SaveContext.SaveFile, ref m_SaveFile);

                WriteToDisk(m_SaveFile);

                SavingCompleted?.Invoke(SaveContext.SaveFile, m_SaveFile);
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
            lock(m_SaveFileLock)
            {
                //TODO: add this for implementing quicksaves/hardsaves
                //ReadFromDisk();

                m_SwitchStates = sf.switchStates;

                Loading?.Invoke(SaveContext.SaveFile, sf);

                LoadingCompleted?.Invoke(SaveContext.SaveFile, sf);
            }
        }

        // PROTECTED METHODS

        protected virtual string GetES3FilePath(int saveSlotIndex = 0)
        {
            if (saveSlotIndex == 0)
            {
                saveSlotIndex = SaveSlot;
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