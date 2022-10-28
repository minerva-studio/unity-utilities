using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentAttributePropertyDrawer : PropertyDrawer
    {
        IndentAttribute Attribute => (IndentAttribute)attribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.indentLevel += Attribute.indent;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }


}