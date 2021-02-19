using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    [CustomPropertyDrawer(typeof(BoolObject), true)]
    public class BoolObjectDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float wBool = 15;
            const float wOEnd = 14;

            //for some reason, the label is lacking the tooltip from TooltipAttribute, so add it here
            var tt = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), true).FirstOrDefault()
            as TooltipAttribute;
            if (tt != null)
            {
                label.tooltip = tt.tooltip;
            }

            //override the label text if applicable
            var lb = fieldInfo.GetCustomAttributes(typeof(LabelTextAttribute), true).FirstOrDefault()
            as LabelTextAttribute;
            if (lb != null)
            {
                label.text = lb.labelText;
            }

            EditorGUI.BeginProperty(position, label, property);

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - wBool, position.height);
            EditorGUI.LabelField(labelRect, label);

            Rect boolRect = new Rect(labelRect);
            boolRect.x += boolRect.width;
            boolRect.width = wBool;
            SerializedProperty boolProp = property.FindPropertyRelative("_bool");
            EditorGUI.PropertyField(boolRect, boolProp, GUIContent.none);

            Rect objectRect = new Rect(boolRect);
            objectRect.x += wBool;
            objectRect.width = position.width - objectRect.x + wOEnd;
            SerializedProperty objectProp = property.FindPropertyRelative(GetObjectRelativePropertyPath(property));
            DrawObjectField(boolProp.boolValue, objectRect, objectProp);

            EditorGUI.EndProperty();

            //and for some reason, if the tooltip isn't erased here, it will show up on completely unrelated properties
            label.tooltip = string.Empty;
        }

        protected virtual void DrawObjectField(bool boolPropValue, Rect objectRect, SerializedProperty objectProp)
        {
            EditorGUI.PropertyField(objectRect, objectProp, GUIContent.none);
        }

        static string GetObjectRelativePropertyPath(SerializedProperty property)
        {
            switch (property.type)
            {
                case "BoolFloat":
                    return "_float";
                case "BoolInt":
                    return "_int";
                default:
                    G.U.Err("Unsupported property type: " + property.type);
                    return null;
            }
        }
    }
}