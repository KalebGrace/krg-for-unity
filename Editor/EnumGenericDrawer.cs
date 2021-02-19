using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    [CustomPropertyDrawer(typeof(EnumGeneric))]
    public class EnumGenericDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty stringTypeProp = property.FindPropertyRelative("_stringType");
            SerializedProperty intValueProp = property.FindPropertyRelative("_intValue");

            Rect rect = new Rect(position.x, position.y, position.width, position.height);

            label.text = SwapLabelText(label.text);

            string stringType = stringTypeProp.stringValue;
            if (SwapEnum(ref stringType))
            {
                stringTypeProp.stringValue = stringType;
            }

            System.Enum selected = EnumGeneric.ToEnum(stringType, intValueProp.intValue);
            selected = EditorGUI.EnumPopup(rect, label, selected);
            intValueProp.intValue = System.Convert.ToInt32(selected);

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Swaps the enum type used to draw this EnumGeneric instance. Will retain the underlying int value.
        /// </summary>
        /// <returns><c>true</c>, if enum was swapped, <c>false</c> otherwise.</returns>
        /// <param name="enumType">A string representation of the enum type, complete with namespace.</param>
        protected virtual bool SwapEnum(ref string enumType)
        {
            string ans = G.config.ApplicationNamespace;
            enumType = enumType.Replace("_0G.Legacy.", ans + ".");
            return true;
        }

        protected virtual string SwapLabelText(string text)
        {
            switch (text)
            {
                case "Time Thread Index":
                    return "Time Thread";
                default:
                    return text;
            }
        }
    }
}