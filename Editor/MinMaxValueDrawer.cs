using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxValue))]
    public class MinMaxValueDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Return the height of a single line
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                EditorGUI.LabelField(position, "-");
                return;
            }

            var randomizeProp = property.FindPropertyRelative(nameof(MinMaxValue.randomize));
            var valueProp = property.FindPropertyRelative(nameof(MinMaxValue.value));
            var rangeProp = property.FindPropertyRelative(nameof(MinMaxValue.range));

            const float toggleWidth = 15f;
            const float padding = 2f;

            Rect valueRect;

            // Draw the label and get the content rect for values
            if (label != GUIContent.none)
            {
                var contentRect = EditorGUI.PrefixLabel(position, label);
                valueRect = new Rect(contentRect.x, contentRect.y, contentRect.width - toggleWidth - padding, contentRect.height);
            }
            else
            {
                valueRect = new Rect(position.x, position.y, position.width - toggleWidth - padding, position.height);
            }

            Rect toggleRect = new Rect(valueRect.xMax + padding, position.y, toggleWidth, position.height);

            if (randomizeProp != null && randomizeProp.boolValue && rangeProp != null)
            {
                var minProp = rangeProp.FindPropertyRelative(nameof(Range.min));
                var maxProp = rangeProp.FindPropertyRelative(nameof(Range.max));

                if (minProp != null && maxProp != null)
                {
                    float halfWidth = (valueRect.width - padding) * 0.5f;
                    Rect minRect = new Rect(valueRect.x, valueRect.y, halfWidth, valueRect.height);
                    Rect separatorRect = new Rect(valueRect.x + halfWidth, valueRect.y, 15f, valueRect.height);
                    Rect maxRect = new Rect(valueRect.x + halfWidth + padding, valueRect.y, halfWidth, valueRect.height);

                    float min = minProp.floatValue;
                    float newMin = EditorGUI.FloatField(minRect, min);
                    if (newMin != min) minProp.floatValue = newMin;

                    EditorGUI.LabelField(separatorRect, "-", EditorStyles.centeredGreyMiniLabel);

                    float max = maxProp.floatValue;
                    float newMax = EditorGUI.FloatField(maxRect, max);
                    if (newMax != max) maxProp.floatValue = newMax;
                }
            }
            else if (valueProp != null)
            {
                float value = valueProp.floatValue;
                float newValue = EditorGUI.FloatField(valueRect, value);
                if (newValue != value) valueProp.floatValue = newValue;
            }

            if (randomizeProp != null)
            {
                DrawModeDropdown(toggleRect, property.serializedObject, randomizeProp, randomizeProp.boolValue);
            }
        }

        private static void DrawModeDropdown(Rect dropdownRect, SerializedObject serializedObject, SerializedProperty randomizeProp, bool isRandomized)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.label.Draw(dropdownRect, GUIContent.none, false, false, false, false);
                GUI.Label(dropdownRect, EditorGUIUtility.IconContent("Icon Dropdown"), EditorStyles.miniLabel);
            }

            if (GUI.Button(dropdownRect, GUIContent.none, GUIStyle.none))
            {
                ShowRandomizationMenu(serializedObject, randomizeProp, isRandomized);
            }
        }

        private static void ShowRandomizationMenu(SerializedObject serializedObject, SerializedProperty randomizeProp, bool isRandomized)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Constant"), !isRandomized, () => SetRandomized(serializedObject, randomizeProp, false));
            menu.AddItem(new GUIContent("Random Range"), isRandomized, () => SetRandomized(serializedObject, randomizeProp, true));

            menu.ShowAsContext();
        }

        private static void SetRandomized(SerializedObject serializedObject, SerializedProperty randomizedProperty, bool enabled)
        {
            if (randomizedProperty == null)
            {
                return;
            }

            randomizedProperty.boolValue = enabled;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
