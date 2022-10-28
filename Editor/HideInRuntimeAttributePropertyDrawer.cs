using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(HideInRuntimeAttribute))]
    public class HideInRuntimeAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Application.isPlaying ? 0 : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!Application.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }

}