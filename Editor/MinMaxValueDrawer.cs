using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    //[CustomPropertyDrawer(typeof(MinMaxValue))]
    public class MinMaxValueDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (property.isExpanded ? 3 : 1) * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxValue minMaxValue;
            SerializedProperty randomized = property.FindPropertyRelative(nameof(minMaxValue.randomize));
            SerializedProperty value = property.FindPropertyRelative(nameof(minMaxValue.value));
            SerializedProperty range = property.FindPropertyRelative(nameof(minMaxValue.range));

            Rect singleLine = position;
            singleLine.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(singleLine, property.isExpanded, label);
            EditorGUI.indentLevel++;

            if (property.isExpanded)
            {
                singleLine.x += EditorGUIUtility.singleLineHeight;
                randomized.boolValue = EditorGUI.Toggle(singleLine, randomized.boolValue);
                singleLine.x += EditorGUIUtility.singleLineHeight;
                if (randomized.boolValue)
                {
                    value.floatValue = EditorGUI.FloatField(singleLine, new GUIContent("Value"), value.floatValue);
                }
                else
                {
                    new RangeDrawer().OnGUI(singleLine, range, new GUIContent("Range"));
                }
            }
            EditorGUI.indentLevel--;
            EditorGUI.EndFoldoutHeaderGroup();
        }
    }
}
