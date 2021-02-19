using UnityEngine;
using UnityEngine.Serialization;

namespace _0G.Legacy
{
    public abstract class Comment : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        [FormerlySerializedAs("m_comment")]
        string _comment;
    }
}