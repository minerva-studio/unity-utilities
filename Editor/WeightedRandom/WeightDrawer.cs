using Minerva.Module.WeightedRandom;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor.WeightedRandom
{
    [CustomPropertyDrawer(typeof(Weight<>))]
    [CustomPropertyDrawer(typeof(Weight))]
    public class WeightDrawer : PropertyDrawer
    {
        const int WEIGHT_LABEL_WIDTH = 100;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var weightPos = position;
            weightPos.width = WEIGHT_LABEL_WIDTH;
            var itemPos = position;
            itemPos.x += WEIGHT_LABEL_WIDTH + 10;
            itemPos.width = position.width - WEIGHT_LABEL_WIDTH - 10;

            var currentWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            EditorGUI.PropertyField(weightPos, property.FindPropertyRelative(nameof(Weight.weight)));
            EditorGUIUtility.labelWidth = 30;
            EditorGUI.PropertyField(itemPos, property.FindPropertyRelative(nameof(Weight.item)));
            EditorGUIUtility.labelWidth = currentWidth;
        }

    }
}
