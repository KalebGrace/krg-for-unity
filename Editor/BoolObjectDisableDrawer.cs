using UnityEditor;
using UnityEngine;

namespace _0G.Legacy
{
    [CustomPropertyDrawer(typeof(BoolObjectDisableAttribute))]
    public class BoolObjectDisableDrawer : BoolObjectDrawer
    {
        #region protected methods

        protected override void DrawObjectField(bool boolPropValue, Rect objectRect, SerializedProperty objectProp)
        {
            var attr = attribute as BoolObjectDisableAttribute;
            string des = attr.disableDescription;
            bool atv = attr.boolValue;
            bool isDisabled = (atv && boolPropValue) || (!atv && !boolPropValue);

            if (!isDisabled || string.IsNullOrEmpty(des))
            {
                EditorGUI.BeginDisabledGroup(isDisabled);
                EditorGUI.PropertyField(objectRect, objectProp, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                const float w = 44;

                Rect desRect = new Rect(objectRect);

                objectRect.width = w;

                desRect.x += w;
                desRect.width -= w;

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(objectRect, objectProp, GUIContent.none);
                EditorGUI.EndDisabledGroup();

                EditorGUI.LabelField(objectRect, "-------");

                EditorGUI.LabelField(desRect, "(" + des + ")");
            }
        }

        #endregion
    }
}