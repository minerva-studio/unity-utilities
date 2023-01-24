using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(RangeInt))]
    public class RangeIntDrawer : PropertyDrawer
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
            float labelWidth = EditorGUIUtility.labelWidth;
            position.x += labelWidth;
            position.width -= labelWidth;
            Draw(position, property);
            EditorGUI.EndProperty();
        }

        public static void Draw(Rect position, SerializedProperty property)
        {
            RangeInt range;
            SerializedProperty minP = property.FindPropertyRelative(nameof(range.min));
            SerializedProperty maxP = property.FindPropertyRelative(nameof(range.max));
            int[] arr = new int[2];
            arr[0] = minP.intValue;
            arr[1] = maxP.intValue;
            EditorGUI.MultiIntField(position, new GUIContent[] { new GUIContent("Min"), new GUIContent("Max") }, arr);

            minP.intValue = arr[0];
            maxP.intValue = arr[1];

            if (minP.intValue > maxP.intValue)
            {
                maxP.intValue = minP.intValue;
            }
            if (maxP.intValue < minP.intValue)
            {
                minP.intValue = maxP.intValue;
            }
        }
    }
}
