using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(Range))]
    public class RangeDrawer : PropertyDrawer
    {
        private float PropertyHeight => EditorGUIUtility.singleLineHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect singleProperty = position;
            singleProperty.width = EditorGUIUtility.labelWidth;
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(singleProperty, label);
            Draw(position, property);
            EditorGUI.EndProperty();
        }

        public static void Draw(Rect position, SerializedProperty property)
        {
            SerializedProperty minP = property.FindPropertyRelative(nameof(Range.min));
            SerializedProperty maxP = property.FindPropertyRelative(nameof(Range.max));
            float[] arr = new float[2];
            arr[0] = minP.floatValue;
            arr[1] = maxP.floatValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            position.x += labelWidth;
            position.width -= labelWidth;
            EditorGUI.MultiFloatField(position, new GUIContent[] { new GUIContent("Min"), new GUIContent("Max") }, arr);

            minP.floatValue = arr[0];
            maxP.floatValue = arr[1];

            if (minP.floatValue > maxP.floatValue)
            {
                maxP.floatValue = minP.floatValue;
            }
            if (maxP.floatValue < minP.floatValue)
            {
                minP.floatValue = maxP.floatValue;
            }
        }
    }
}
