using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(UUID))]
    public class UUIDEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty valueProperty = property.FindPropertyRelative("value");
            EditorGUI.PropertyField(position, valueProperty, label);
            property.boxedValue = new UUID(valueProperty.stringValue);
            EditorGUI.EndProperty();
        }
    }
}