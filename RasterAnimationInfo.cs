using TMPro;
using UnityEngine;

namespace _0G.Legacy
{
    [System.Obsolete]
    public class RasterAnimationInfo : MonoBehaviour
    {
        const string _infoFormat =
            "{0}\n" + //name
            "f: {1} ({2}) {3}\n" + //   from:    f:min (as if inclusive), f:random, f:max (as if inclusive)
            "t: {4} ({5}) {6}\n" + //    to:     t:min (as if inclusive), t:random, t:max (as if inclusive)
            "p: {7} ({8}) {9}\n" + //play count: p:min (as if inclusive), p:random, p:max (as if inclusive)
            "{10}/{11}s {12}/{13}p\n" + //frame sequence current/count, play current/count, frame number/to
            "{14}/{15}t";

        TextMeshPro _text;

        public string frameSequenceName { get; set; }

        //f:
        public int frameSequenceFromFrameMin { get; set; }

        public int frameSequenceFromFrame { get; set; }

        public int frameSequenceFromFrameMax { get; set; }

        //t:
        public int frameSequenceToFrameMin { get; set; }

        public int frameSequenceToFrame { get; set; }

        public int frameSequenceToFrameMax { get; set; }

        //p:
        public int frameSequencePlayCountMin { get; set; }

        public int frameSequencePlayCount { get; set; }

        public int frameSequencePlayCountMax { get; set; }

        //#:
        public int frameSequenceCount { get; set; }

        public int frameSequenceIndex { get; set; }

        public int frameSequencePlayIndex { get; set; }

        public int frameNumber { get; set; }

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();

            if (G.U.IsDebugMode)
            {
                G.U.Require(_text, "TextMesh (Pro) Component", gameObject, "this GameObject");
            }
            else
            {
                _text?.Dispose();
            }
        }

        public void Clear()
        {
            if (G.U.IsDebugMode)
            {
                _text.text = "N/A";
            }
        }

        public void Refresh()
        {
            if (G.U.IsDebugMode)
            {
                _text.text = string.Format(
                    _infoFormat,
                    frameSequenceName,
                    frameSequenceFromFrameMin,
                    frameSequenceFromFrame,
                    frameSequenceFromFrameMax,
                    frameSequenceToFrameMin,
                    frameSequenceToFrame,
                    frameSequenceToFrameMax,
                    frameSequencePlayCountMin,
                    frameSequencePlayCount,
                    frameSequencePlayCountMax,
                    frameSequenceIndex + 1,
                    frameSequenceCount,
                    frameSequencePlayIndex + 1,
                    frameSequencePlayCount,
                    frameNumber,
                    frameSequenceToFrame
                );
            }
        }
    }
}