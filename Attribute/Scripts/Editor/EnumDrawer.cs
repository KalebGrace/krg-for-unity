using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KRG {

    /// <summary>
    /// Enum drawer.
    /// Last Refactor: 0.05.002 / 2018-05-05
    /// </summary>
    //[CustomPropertyDrawer(typeof(EnumAttribute))]
    public abstract class EnumDrawer : EnumGenericDrawer {

#region public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = new Rect(position.x, position.y, position.width, position.height);

            label.text = SwapLabelText(label.text);

            var attr = attribute as EnumAttribute;
            string stringType = attr.enumType.ToString();
            SwapEnum(ref stringType);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                System.Enum selected = EnumGeneric.ToEnum(stringType, property.intValue);
                selected = EditorGUI.EnumPopup(rect, label, selected);
                property.intValue = System.Convert.ToInt32(selected);
            }
            else
            {
                G.U.Error("The Enum attribute doesn't have support for the {0} type."
                    + " Property name: {1}. Attribute enum type: {2}.",
                    property.propertyType, property.name, stringType);
                EditorGUI.PropertyField(rect, property, label);
            }

            EditorGUI.EndProperty();
        }

#endregion

    }
}
