using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyIfAttribute))]
    public class ReadOnlyIfAttributeAttributeDrawer : ConditionalFieldAttributeDrawer
    {
        protected override void DrawField(Rect position, SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            if (conditionMatches)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        protected override float GetFieldHeight(SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            return base.GetBasePropertyHeight(property, label);
        }
    }
}