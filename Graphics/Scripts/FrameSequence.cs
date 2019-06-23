using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace KRG {

    [System.Serializable]
    public sealed class FrameSequence {

        class FrameCommand {
            public bool isNumber, isExtender, isRange, isSeparator; //TODO: make this an enum
            public int number, times = 1;
        }

#region CONSTANTS

        const int _version = 1;

#endregion

#region FIELDS: SERIALIZED

        //TODO: display the following information in the UI
        //IMPORTANT: frame sequences can't use both "Frames" and "From/To Frame" in the same animation
        //- must be all one or all the other

        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_serializedVersion")]
        int _serializedVersion = default;

        [SerializeField]
        [Tooltip("An optional name you may give this frame sequence.")]
        [FormerlySerializedAs("m_name")]
        string _name = default;

        [SerializeField]
        [Tooltip("Commas seperate frames/groups. 1-3-1 means 1,2,3,2,1. 1-3x2-1 means 1-3,3-1 means 1,2,3,3,2,1.")]
        string _frames = default;

        //TODO: make w/ odin control
        [System.NonSerialized]
        [Tooltip("This is how the code sees your Frames input.")]
        public string _framesInterpreted;

        [HideInInspector]
        [SerializeField]
        List<int> _frameList = new List<int>();

        [SerializeField]
        [FormerlySerializedAs("m_fromFrame")]
        RangeInt _fromFrame = new RangeInt();

        [SerializeField]
        [FormerlySerializedAs("m_toFrame")]
        RangeInt _toFrame = new RangeInt();

        [SerializeField]
        [FormerlySerializedAs("m_playCount")]
        RangeInt _playCount = new RangeInt();

        [SerializeField]
        [Tooltip("Denotes that this does call code upon starting this frame sequence.")]
        [FormerlySerializedAs("m_doesCallCode")]
        bool _doesCallCode = default;

        //[System.Obsolete("Use _fromFrame instead.")]
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_from")]
        int _from = default;

        //[System.Obsolete("Use _toFrame instead.")]
        [HideInInspector]
        [SerializeField]
        [FormerlySerializedAs("m_to")]
        int _to = default;

#endregion

#region FIELDS: PRIVATE / ConvertFramesToFrameList

        Queue<FrameCommand> _frameCommands = new Queue<FrameCommand>();

        StringBuilder _number;

#endregion

#region PROPERTIES

        public string name { get { return _name; } }

        public ReadOnlyCollection<int> frameList { get { return _frameList.AsReadOnly(); } }

        public int fromFrame { get { return _fromFrame.randomValue; } }

        public bool fromFrameMinInclusive { get { return _fromFrame.minInclusive; } }

        public bool fromFrameMaxInclusive { get { return _fromFrame.maxInclusive; } }

        public int fromFrameMinValue { get { return _fromFrame.minValue; } }

        public int fromFrameMaxValue { get { return _fromFrame.maxValue; } }

        public int toFrame { get { return _toFrame.randomValue; } }

        public bool toFrameMinInclusive { get { return _toFrame.minInclusive; } }

        public bool toFrameMaxInclusive { get { return _toFrame.maxInclusive; } }

        public int toFrameMinValue { get { return _toFrame.minValue; } }

        public int toFrameMaxValue { get { return _toFrame.maxValue; } }

        public int playCount { get { return _playCount.randomValue; } }

        public bool playCountMinInclusive { get { return _playCount.minInclusive; } }

        public bool playCountMaxInclusive { get { return _playCount.maxInclusive; } }

        public int playCountMinValue { get { return _playCount.minValue; } }

        public int playCountMaxValue { get { return _playCount.maxValue; } }

        public bool doesCallCode { get { return _doesCallCode; } }

#endregion

#region METHODS: PUBLIC

        public void OnValidate() {
            //init version / migrate deprecated values
            if (_serializedVersion < _version) {
                //initialization
                if (_playCount.minValue == 0 && _playCount.maxValue == 0) {
                    _playCount.minValue = 1;
                }
                //version 0
                if (_from > 0) {
                    _fromFrame.minValue = _from;
                    _fromFrame.maxValue = _from;
                    _from = 0;
                }
                if (_to > 0) {
                    _toFrame.minValue = _to;
                    _toFrame.maxValue = _to;
                    _to = 0;
                }
                //version 1
                _serializedVersion = _version;
            }
            //real validation
            _fromFrame.minValue = Mathf.Max(1, _fromFrame.minValue);
            _toFrame.minValue = Mathf.Max(_fromFrame.maxValue, _toFrame.minValue);
            _playCount.minValue = Mathf.Max(0, _playCount.minValue);

            ConvertFramesToFrameList();
        }

#endregion

#region METHODS: ConvertFramesToFrameList

        void ConvertFramesToFrameList() {
            _frameList.Clear();
            _frameCommands.Clear();
            _number = new StringBuilder();
            _frames = _frames?.Trim() ?? "";
            if (_frames == "") return;
            char c;
            for (int i = 0; i < _frames.Length; ++i) {
                c = _frames[i];
                switch (c) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        _number.Append(c);
                        break;
                    case 'x':
                        FlushNumberToFrameCommands();
                        _frameCommands.Enqueue(new FrameCommand { isExtender = true });
                        break;
                    case 't': // to
                    case '-':
                        FlushNumberToFrameCommands();
                        _frameCommands.Enqueue(new FrameCommand { isRange = true });
                        break;
                    case ',':
                        FlushNumberToFrameCommands();
                        _frameCommands.Enqueue(new FrameCommand { isSeparator = true });
                        break;
                }
            }
            if (_number.Length > 0) FlushNumberToFrameCommands();
            ProcessFrameCommandExtenders();
            ProcessFrameCommandRanges();
        }

        void FlushNumberToFrameCommands() {
            if (_number.Length > 0) {
                _frameCommands.Enqueue(new FrameCommand { isNumber = true, number = int.Parse(_number.ToString()) });
                _number = new StringBuilder();
            } else {
                Error("Did you forget a number before/after a symbol?"
                + " Number string is null or empty in AddNumberToCommands.");
            }
        }

        void ProcessFrameCommandExtenders() {
            var q = _frameCommands; //the original queue of frame commands
            _frameCommands = new Queue<FrameCommand>(); //the new, processed queue of frame commands
            FrameCommand prev = null, curr, next; //previous, current, next
            while (q.Count > 0) {
                curr = q.Dequeue();
                if (curr.isExtender) {
                    if (IsBinaryOperator(prev, q, out next)) {
                        prev.times *= next.number;
                    }
                } else {
                    //this will normally be done first, before curr.isExtender
                    _frameCommands.Enqueue(curr);
                    prev = curr;
                }
            }
        }

        void ProcessFrameCommandRanges() {
            var q = _frameCommands; //the original queue of frame commands
            //this is the last process, so we're gonna affect the original queue this time
            FrameCommand prev = null, curr, next; //previous, current, next
            while (q.Count > 0) {
                curr = q.Dequeue();
                if (curr.isRange) {
                    if (IsBinaryOperator(prev, q, out next)) {
                        AddFramesToList(prev, next);
                        prev = next;
                    }
                } else if (curr.isNumber) {
                    //this will normally be done first, before curr.isRange or curr.isSeparator
                    AddFrameToList(curr);
                    prev = curr;
                } else if (curr.isSeparator) {
                    //do nothing
                    prev = null;
                } else {
                    Error("Unrecognized symbol. Should only be a number,"
                    + " an extender (x), a range dash (-), or a separator comma (,).");
                }
            }
        }

        static bool IsBinaryOperator(FrameCommand prev, Queue<FrameCommand> q, out FrameCommand next) {
            next = null;
            if (q.Count == 0) {
                Error("Missing right operator at end.");
                return false;
            }
            if (!q.Peek().isNumber) { //peek at next, but don't dequeue it yet
                Error("Missing right operator.");
                return false;
            }
            next = q.Dequeue(); //dequeue next, and set _out_ parameter
            if (prev == null) {
                Error("Missing left operator at beginning.");
                return false;
            }
            if (!prev.isNumber) {
                Error("Missing left operator.");
                return false;
            }
            return true;
        }

        void AddFramesToList(FrameCommand fromEx, FrameCommand toIncl) { //_from_ exclusive, _to_ inclusive
            if (fromEx.number == toIncl.number) {
                Error("Same _from_ and _to_. Ignoring second number. Use a comma if you want it twice.");
                return;
            }
            //add all the numbers between _from_ and _to_
            if (fromEx.number < toIncl.number) {
                for (int i = fromEx.number + 1; i < toIncl.number; ++i) _frameList.Add(i);
            } else {
                for (int i = fromEx.number - 1; i > toIncl.number; --i) _frameList.Add(i);
            }
            //and then add _to_ using its specified _times_ (this way we can process e.g. 1-5x0 as 1,2,3,4)
            AddFrameToList(toIncl);
        }

        void AddFrameToList(FrameCommand c) {
            while (c.times-- > 0) _frameList.Add(c.number);
        }

        static void Error(string message) {
            //TODO: add this into the GUI instead
            G.U.Err("Error in FrameSequence. {0}", message);
        }

#endregion

    }
}
