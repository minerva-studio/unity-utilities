using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(DisplayIfAttribute))]
    public class DisplayIfAttributeDrawer : ConditionalFieldAttributeDrawer
    {
        protected override float GetFieldHeight(SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            if (conditionMatches)
            {
                return GetBasePropertyHeight(property, label);
            }
            else return 0;
        }

        protected override void DrawField(Rect position, SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            if (conditionMatches) EditorGUI.PropertyField(position, property, label, true);
        }
    }
}