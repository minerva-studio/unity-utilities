using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxValue))]
    public class MinMaxValueDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
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

            Rect valueRect = new Rect(position.x + padding, position.y, position.width - toggleWidth, position.height - padding * 2);
            Rect toggleRect = new Rect(valueRect.x + valueRect.width - padding, position.y, toggleWidth, valueRect.height);

            if (randomizeProp != null && randomizeProp.boolValue && rangeProp != null)
            {
                var minProp = rangeProp.FindPropertyRelative(nameof(Range.min));
                var maxProp = rangeProp.FindPropertyRelative(nameof(Range.max));

                if (minProp != null && maxProp != null)
                {
                    float halfWidth = (valueRect.width - 15f) * 0.5f;
                    Rect minRect = new Rect(valueRect.x, valueRect.y, halfWidth, valueRect.height);
                    Rect separatorRect = new Rect(valueRect.x + halfWidth, valueRect.y, 15f, valueRect.height);
                    Rect maxRect = new Rect(valueRect.x + halfWidth + 15f, valueRect.y, halfWidth, valueRect.height);

                    minProp.floatValue = EditorGUI.FloatField(minRect, label, minProp.floatValue);
                    EditorGUI.LabelField(separatorRect, "-", EditorStyles.centeredGreyMiniLabel);
                    maxProp.floatValue = EditorGUI.FloatField(maxRect, maxProp.floatValue);
                }
            }
            else if (valueProp != null)
            {
                valueProp.floatValue = EditorGUI.FloatField(valueRect, label, valueProp.floatValue);
            }

            if (randomizeProp != null)
            {
                DrawModeDropdown(toggleRect, property.serializedObject, randomizeProp, randomizeProp.boolValue);
            }
        }

        private static void DrawModeDropdown(Rect dropdownRect, SerializedObject serializedObject, SerializedProperty randomizeProp, bool isRandomized)
        {
            // Overlay a clickable hotspot on top of the value field without drawing a background button.
            // Draw a small arrow to hint interactivity.
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