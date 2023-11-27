using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(Counter))]
    public class CounterEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var state = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.IntField(position, label, (property.GetValue() as Counter).Count);
            GUI.enabled = state;
            EditorGUI.EndProperty();
        }
    }
}