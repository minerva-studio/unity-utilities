using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorFieldDrawers.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object value = property.GetValue();
            if (value is string str) EditorGUI.LabelField(position, label.text, str);
            else { EditorFieldDrawers.PropertyField(position, property, label, true); }
        }
    }

}