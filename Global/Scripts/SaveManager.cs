namespace KRG
{
    public class SaveManager : Manager
    {
        public override float priority => 20;


        // FIELDS

        private SaveFile m_CurrentCheckpoint;


        // MONOBEHAVIOUR-LIKE METHODS

        public override void Awake()
        {
#if KRG_X_EASY_SAVE_3
            m_CurrentCheckpoint = ES3.Load("CheckpointSaveFile", SaveFile.New(SaveContext.ContinueCheckpoint));
#endif
        }


        // PUBLIC METHODS

        public void SaveCheckpoint()
        {
            m_CurrentCheckpoint = SaveFile.New(SaveContext.ContinueCheckpoint);
            G.instance.InvokeManagers<ISave>(i => i.SaveTo(ref m_CurrentCheckpoint));

#if KRG_X_EASY_SAVE_3
            ES3.Save<SaveFile>("CheckpointSaveFile", m_CurrentCheckpoint);
#endif
        }

        public void LoadCheckpoint()
        {
            G.instance.InvokeManagers<ISave>(i => i.LoadFrom(m_CurrentCheckpoint));
        }
    }
}
