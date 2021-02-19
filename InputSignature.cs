using System.Collections.Generic;
using UnityEngine;

namespace _0G.Legacy
{
    public abstract class InputSignature : ScriptableObject
    {
        [Header("Optional Easy Access Key")]

        [SerializeField, Enum(typeof(InputEzKey))]
        protected int m_EzKey;

        [Header("Input Signature Elements")]

        [SerializeField]
        protected List<InputSignatureElement> m_Elements;

        string m_Key;

        public virtual int complexity { get { return m_Elements != null ? m_Elements.Count : 0; } }

        public virtual int ezKey { get { return m_EzKey; } }

        public virtual bool hasEzKey { get { return m_EzKey != 0; } }

        /// <summary>
        /// Gets the key, a unique identifier. It will be cached upon first acesss.
        /// </summary>
        /// <value>The key.</value>
        public virtual string key
        {
            get
            {
                if (string.IsNullOrEmpty(m_Key))
                {
                    m_Key = m_EzKey.ToString();

                    if (!hasEzKey && G.U.Assert(m_Elements != null))
                    {
                        int len = m_Elements.Count;
                        if (G.U.Assert(len > 0))
                        {
                            for (int i = 0; i < len; ++i)
                            {
                                var e = m_Elements[i];
                                m_Key += "+" + e.inputCommand;
                            }
                        }
                    }
                }
                return m_Key;
            }
        }

        public abstract bool IsExecuted(Attacker attacker);
    }
}