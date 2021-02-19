using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    [CustomPropertyDrawer(typeof(RangeFloat))]
    [CustomPropertyDrawer(typeof(RangeInt))]
    public class RangeObjectDrawer : PropertyDrawer
    {
        SerializedProperty _minInclusiveProp, _minValueProp, _maxValueProp, _maxInclusiveProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float wInclusive = 15;

            EditorGUI.BeginProperty(position, label, property);

            _minInclusiveProp = property.FindPropertyRelative("_minInclusive");
            _minValueProp = property.FindPropertyRelative("_minValue");
            _maxValueProp = property.FindPropertyRelative("_maxValue");
            _maxInclusiveProp = property.FindPropertyRelative("_maxInclusive");

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            AddSummary(labelRect);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float wValue = (position.width - labelRect.width - (2f * wInclusive)) / 2f;

            Rect minInclusiveRect = AddField(_minInclusiveProp, wInclusive, labelRect);
            Rect minValueRect = AddField(_minValueProp, wValue, minInclusiveRect);
            Rect maxValueRect = AddField(_maxValueProp, wValue, minValueRect);
            AddField(_maxInclusiveProp, wInclusive, maxValueRect);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        static Rect AddField(SerializedProperty property, float width, Rect previousField)
        {
            Rect r = new Rect(previousField);
            r.x += r.width;
            r.width = width;
            EditorGUI.PropertyField(r, property, GUIContent.none);
            return r;
        }

        void AddSummary(Rect overlayField)
        {
            Rect r = new Rect(overlayField);

            GUIStyle sameStyle = new GUIStyle(EditorStyles.whiteBoldLabel);
            sameStyle.alignment = TextAnchor.MiddleRight;

            GUIStyle diffStyle = new GUIStyle(EditorStyles.miniLabel);
            diffStyle.alignment = TextAnchor.MiddleRight;
            diffStyle.normal.textColor = Color.red;

            GUIStyle currStyle = null;
            string summary = "";

            switch (_minValueProp.type)
            {
                case "float":
                    if (Mathf.Approximately(_minValueProp.floatValue, _maxValueProp.floatValue))
                    {
                        currStyle = sameStyle;
                        summary = _minValueProp.floatValue.ToString();
                    }
                    else
                    {
                        currStyle = diffStyle;
                        const string f = "#0.#";
                        string min = _minValueProp.floatValue.ToString(f) + (_minInclusiveProp.boolValue ? "" : "*");
                        string max = _maxValueProp.floatValue.ToString(f) + (_maxInclusiveProp.boolValue ? "" : "*");
                        summary = min + "~" + max;
                    }
                    break;
                case "int":
                    if (_minValueProp.intValue == _maxValueProp.intValue)
                    {
                        currStyle = sameStyle;
                        summary = _minValueProp.intValue.ToString();
                    }
                    else
                    {
                        currStyle = diffStyle;
                        int min = _minValueProp.intValue + (_minInclusiveProp.boolValue ? 0 : 1);
                        int max = _maxValueProp.intValue - (_maxInclusiveProp.boolValue ? 0 : 1);
                        summary = min + "~" + max;
                    }
                    break;
                default:
                    G.U.Err("Invalid type: " + _minValueProp.type);
                    break;
            }

            EditorGUI.LabelField(r, summary, currStyle);
        }
    }
}