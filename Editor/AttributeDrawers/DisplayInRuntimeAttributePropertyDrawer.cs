using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(DisplayInRuntimeAttribute))]
    public class DisplayInRuntimeAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return !Application.isPlaying ? 0 : EditorFieldDrawers.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                EditorFieldDrawers.PropertyField(position, property, label, true);
            }
        }
    }

}