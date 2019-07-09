namespace KRG
{
    public class SaveManager : Manager
    {
        public override float priority => 20;


        // FIELDS

        private SaveFile m_CurrentCheckpoint;

        private object m_SaveLock = new object();


        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
#if KRG_X_EASY_SAVE_3
            m_CurrentCheckpoint = ES3.Load("CheckpointSaveFile", SaveFile.New(SaveContext.ContinueCheckpoint));
#endif
        }


        // PUBLIC METHODS

        public bool IsCurrentCheckpoint(LetterName checkpointName)
        {
            return (int)checkpointName == m_CurrentCheckpoint.checkpointId &&
                G.app.GameplaySceneId == m_CurrentCheckpoint.gameplaySceneId;
        }

        public void SaveCheckpoint()
        {
            lock (m_SaveLock)
            {
                //no checkpoint specified, so keep previous data
                SaveCheckpoint(m_CurrentCheckpoint.gameplaySceneId, m_CurrentCheckpoint.checkpointId);
            }
        }

        public void SaveCheckpoint(LetterName checkpointName)
        {
            lock (m_SaveLock)
            {
                //checkpoint specified; use current scene as well
                SaveCheckpoint(G.app.GameplaySceneId, (int)checkpointName);
            }
        }

        private void SaveCheckpoint(int gameplaySceneId, int checkpointId)
        {
            m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);
            m_CurrentCheckpoint.gameplaySceneId = gameplaySceneId;
            m_CurrentCheckpoint.checkpointId = checkpointId;

            var pc = PlayerCharacter.instance;
            if (pc != null)
            {
                m_CurrentCheckpoint.position = pc.transform.position; //for logging only
            }

            G.instance.InvokeManagers<ISave>(i => i.SaveTo(ref m_CurrentCheckpoint));

#if KRG_X_EASY_SAVE_3
            ES3.Save<SaveFile>("CheckpointSaveFile", m_CurrentCheckpoint);
#endif
        }

        public void LoadCheckpoint()
        {
            lock (m_SaveLock)
            {
                G.app.GameplaySceneId = m_CurrentCheckpoint.gameplaySceneId;

                //TODO: set player position based on checkpoint id

                G.instance.InvokeManagers<ISave>(i => i.LoadFrom(m_CurrentCheckpoint));
            }
        }
    }
}
