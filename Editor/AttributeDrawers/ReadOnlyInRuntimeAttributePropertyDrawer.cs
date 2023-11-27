using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyInRuntimeAttribute))]
    public class ReadOnlyInRuntimeAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorFieldDrawers.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                var prev = GUI.enabled;
                GUI.enabled = false;
                EditorFieldDrawers.PropertyField(position, property, label, true);
                GUI.enabled = prev;
            }
            else
            {
                EditorFieldDrawers.PropertyField(position, property, label, true);
            }
        }
    }

}