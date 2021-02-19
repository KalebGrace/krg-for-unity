using UnityEngine;

namespace _0G.Legacy
{
    public class PersistentData<T>
    {
        public delegate void Handler(PersistentData<T> persistentData);

        public static event Handler Initialized;
        public static event Handler Disposed;

        // CONSTRUCTOR / DESTRUCTOR

        public PersistentData(
            Persist persist,
            string key,
            T defaultValue,
            int loadState = 0,
            ValueChangedHandler valueChangedHandler = null)
        {
            Persist = persist;
            Key = key;
            Type = typeof(T);
            Value = defaultValue;
            ValueChanged = valueChangedHandler;

            // TODO: apply load state as needed

            switch (Persist)
            {
                case Persist.PlayerPrefs:
                    G.DoPlayerPrefsAction(ReadFromPlayerPrefs);
                    break;
                case Persist.PlayerPrefs_Overwrite:
                    G.DoPlayerPrefsAction(WriteToPlayerPrefs);
                    break;
            }

            switch (Persist)
            {
                case Persist.PlayerPrefs:
                case Persist.PlayerPrefs_Overwrite:
                    ValueChanged += WriteToPlayerPrefs;
                    break;
            }

            Initialized?.Invoke(this);
        }

        ~PersistentData()
        {
            Disposed?.Invoke(this);

            ValueChanged = null;
        }

        // PERSIST

        public Persist Persist;

        // KEY

        public string Key;

        // TYPE

        public System.Type Type;

        // VALUE

        public delegate void ValueChangedHandler(PersistentData<T> persistentData, T oldValue, T newValue);

        public event Handler ValueRequested;
        public event ValueChangedHandler ValueChanged;

        public object m_Value;

        public T Value
        {
            get
            {
                ValueRequested?.Invoke(this);
                return (T) m_Value;
            }
            set
            {
                object oldValue = m_Value;
                m_Value = value;
                ValueChanged?.Invoke(this, (T) oldValue, value);
            }
        }

        // PLAYER PREFS

        public bool IsReadingPlayerPrefs { get; private set; }

        private static void WriteToPlayerPrefs(PersistentData<T> persistentData, T oldValue, T newValue)
        {
            if (!persistentData.IsReadingPlayerPrefs)
            {
                persistentData.WriteToPlayerPrefs();
            }
        }
        public void WriteToPlayerPrefs()
        {
            switch (m_Value)
            {
                case float f:
                    PlayerPrefs.SetFloat(Key, f);
                    break;
                case int i:
                    PlayerPrefs.SetInt(Key, i);
                    break;
                case bool b:
                    PlayerPrefs.SetInt(Key, b ? 1 : 0);
                    break;
                case string s:
                    PlayerPrefs.SetString(Key, s);
                    break;
                default:
                    PlayerPrefs.SetString(Key, m_Value.ToString());
                    break;
            }
            PlayerPrefs.Save();
        }

        public void ReadFromPlayerPrefs()
        {
            IsReadingPlayerPrefs = true;
            object oldValue = m_Value;
            switch (m_Value)
            {
                case float f:
                    m_Value = PlayerPrefs.GetFloat(Key, f);
                    break;
                case int i:
                    m_Value = PlayerPrefs.GetInt(Key, i);
                    break;
                case bool b:
                    m_Value = PlayerPrefs.GetInt(Key, b ? 1 : 0) > 0;
                    break;
                case string s:
                    m_Value = PlayerPrefs.GetString(Key, s);
                    break;
                default:
                    m_Value = PlayerPrefs.GetString(Key, m_Value?.ToString());
                    break;
            }
            ValueChanged?.Invoke(this, (T) oldValue, (T) m_Value);
            IsReadingPlayerPrefs = false;
        }
    }
}