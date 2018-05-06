using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    public abstract class Comment : MonoBehaviour {

        [SerializeField]
        [TextArea]
        [FormerlySerializedAs("m_comment")]
        string _comment;
    }
}
