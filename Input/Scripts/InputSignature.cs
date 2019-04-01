using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    public abstract class InputSignature : ScriptableObject {

        //TODO: custom "Enum" attribute on an int array doesn't play well w/ Odin Inspector
        [Header("Obsolete KRG InputSignature")]
        //[Enum(typeof(InputCommand))]
        [SerializeField]
        [FormerlySerializedAs("m_inputCommands")]
        protected int[] _inputCommands;

        string _key;

        public virtual int complexity { get { return _inputCommands.Length; } }

        public abstract bool isExecuted { get; }

        /// <summary>
        /// Gets the key, a unique identifier. It will be cached upon first acesss.
        /// </summary>
        /// <value>The key.</value>
        public virtual string key {
            get {
                if (string.IsNullOrEmpty(_key)) {
                    if (G.U.Assert(_inputCommands != null)) {
                        int len = _inputCommands.Length;
                        if (G.U.Assert(len > 0)) {
                            _key = "" + _inputCommands[0];
                            for (int i = 1; i < len; i++) {
                                _key += "+" + _inputCommands[i];
                            }
                        }
                    }
                }
                return _key;
            }
        }
    }
}
