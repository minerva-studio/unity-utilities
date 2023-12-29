using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(Counter))]
    [CustomPropertyDrawer(typeof(Counter<>))]
    public class CounterEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var state = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.IntField(position, label, (property.GetValue() as ICollection)?.Count ?? 0);
            GUI.enabled = state;
            EditorGUI.EndProperty();
        }
    }
}