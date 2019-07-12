namespace KRG
{
    public class SaveManager : Manager
    {
        public override float priority => 5;


        // fields, delegates, events : SAVE FILE

        private SaveFile m_CurrentCheckpoint;

        private readonly object m_SaveLock = new object();

        public delegate void SaveFileWriteHandler(ref SaveFile sf);
        public delegate void SaveFileReadHandler(SaveFile sf);

        private event SaveFileWriteHandler Saving;
        private event SaveFileReadHandler SavingCompleted;
        private event SaveFileReadHandler Loading;
        private event SaveFileReadHandler LoadingCompleted;


        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
#if KRG_X_EASY_SAVE_3
            m_CurrentCheckpoint = ES3.Load("CheckpointSaveFile", SaveFile.New(SaveContext.ContinueCheckpoint));
#endif
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

        public bool IsCurrentCheckpoint(LetterName checkpointName)
        {
            return (int)checkpointName == m_CurrentCheckpoint.checkpointId &&
                 G.app.GameplaySceneId == m_CurrentCheckpoint.gameplaySceneId;
        }

        public void SaveCheckpoint(LetterName checkpointName = 0)
        {
            lock (m_SaveLock)
            {
                m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);

                m_CurrentCheckpoint.checkpointId = (int)checkpointName;

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

                Loading?.Invoke(sf);

                LoadingCompleted?.Invoke(sf);
            }
        }
    }
}
