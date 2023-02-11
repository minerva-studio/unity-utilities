using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxValueInt))]
    public class MinMaxValueIntDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxValueInt minMaxValue;
            SerializedProperty randomized = property.FindPropertyRelative(nameof(minMaxValue.randomize));
            SerializedProperty value = property.FindPropertyRelative(nameof(minMaxValue.value));
            SerializedProperty range = property.FindPropertyRelative(nameof(minMaxValue.range));

            Rect labelRect = position;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(labelRect, label);

            Rect random = position;
            random.x += EditorGUIUtility.labelWidth;
            random.height = EditorGUIUtility.singleLineHeight;
            var currIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.PropertyField(random, randomized, new GUIContent("Randomize"));
            EditorGUI.indentLevel = currIndent;


            Rect valRect = position;
            valRect.height = EditorGUIUtility.singleLineHeight;
            valRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel++;
            if (randomized.boolValue)
            {
                int indent = EditorGUI.indentLevel * 15;
                valRect.x += indent;
                valRect.width -= indent;
                RangeIntDrawer.Draw(valRect, range);
            }
            else
            {
                EditorGUI.PropertyField(valRect, value, true);
            }
            EditorGUI.indentLevel--;

        }
    }
}
