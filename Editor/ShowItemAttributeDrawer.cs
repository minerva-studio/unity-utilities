using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(ShowItemAttribute))]
    public class ShowItemAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var a = (ShowItemAttribute)attribute;
            if (!Exist(property, a.path, out var result)) return base.GetPropertyHeight(property, label);
            return base.GetPropertyHeight(property, label) * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var a = (ShowItemAttribute)attribute;
            position.height += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label);
            if (Exist(property, a.path, out var result))
            {
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, result);
            }
        }


        public static bool Exist(SerializedProperty from, string path, out SerializedProperty serializedProperty)
        {
            var sp = from.FindPropertyRelative(path);
            if (sp != null) { serializedProperty = sp; return true; }

            sp = from.serializedObject.FindProperty(path);
            if (sp != null) { serializedProperty = sp; return true; }

            serializedProperty = null;
            return false;
        }
    }
}