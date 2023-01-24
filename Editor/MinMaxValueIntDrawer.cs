using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    //[CustomPropertyDrawer(typeof(MinMaxValueInt))]
    public class MinMaxValueIntDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxValueInt minMaxValue;
            SerializedProperty randomized = property.FindPropertyRelative(nameof(minMaxValue.randomize));
            SerializedProperty value = property.FindPropertyRelative(nameof(minMaxValue.value));
            SerializedProperty range = property.FindPropertyRelative(nameof(minMaxValue.range));

            Rect singleLine = position;
            singleLine.height = EditorGUIUtility.singleLineHeight;
            float fullWidth = singleLine.width;
            //property.isExpanded = EditorGUI.Foldout(singleLine, property.isExpanded, label);
            Rect labelRect = singleLine;
            labelRect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(singleLine, label);
            EditorGUI.indentLevel++;

            if (property.isExpanded)
            {
                singleLine.x += EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15f;
                singleLine.width = EditorGUIUtility.singleLineHeight * 8;
                EditorGUI.LabelField(singleLine, new GUIContent("Randomize"));

                singleLine.x += EditorGUIUtility.singleLineHeight * 4;
                singleLine.width = EditorGUIUtility.singleLineHeight;
                randomized.boolValue = EditorGUI.Toggle(singleLine, randomized.boolValue);

                singleLine.x += EditorGUIUtility.singleLineHeight * 5;
                singleLine.width = fullWidth - EditorGUIUtility.singleLineHeight * 7 - EditorGUIUtility.labelWidth;
                if (randomized.boolValue)
                {
                    value.intValue = EditorGUI.IntField(singleLine, new GUIContent("Value"), value.intValue);
                }
                else
                {
                    RangeIntDrawer.Draw(singleLine, range);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUI.EndFoldoutHeaderGroup();
        }
    }
}
