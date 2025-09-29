using Minerva.Module.WeightedRandom;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor.WeightedRandom
{
    // Attach to both open-generic and non-generic variants.
    // Enable useForChildren to improve matching reliability across Unity versions.
    [CustomPropertyDrawer(typeof(Weight<>), true)]
    [CustomPropertyDrawer(typeof(Weight), true)]
    public class WeightDrawer : PropertyDrawer
    {
        const int WEIGHT_LABEL_WIDTH = 100;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Single-line height; extend here if you later add foldouts.
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 1) Wrap the whole field with Begin/EndProperty to ensure prefab override,
            // focus, and highlight behaviors work correctly.
            EditorGUI.BeginProperty(position, label, property);

            // 2) Update the serialized object (safer for nesting / multi-object editing).
            var so = property.serializedObject;
            so.Update();

            // 3) Layout
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var weightPos = line; weightPos.width = WEIGHT_LABEL_WIDTH;
            var itemPos = line; itemPos.x += WEIGHT_LABEL_WIDTH + 10;
            itemPos.width = line.width - WEIGHT_LABEL_WIDTH - 10;

            // 4) Fetch child properties (do not cache SerializedProperty references).
            var weightProp = property.FindPropertyRelative("weight");
            var itemProp = property.FindPropertyRelative("item");

            // 5) Change check
            EditorGUI.BeginChangeCheck();

            // Control label width; remember to restore it afterward.
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50f;
            EditorGUI.PropertyField(weightPos, weightProp, new GUIContent("Wt"));

            EditorGUIUtility.labelWidth = 30f;
            EditorGUI.PropertyField(itemPos, itemProp, new GUIContent("Val"), true);

            EditorGUIUtility.labelWidth = oldLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                // 6) Apply serialized changes
                so.ApplyModifiedProperties();

                // 7) Record Undo and mark dirty (helps in arrays/nested structs/multi-edit).
                foreach (var target in so.targetObjects)
                {
                    Undo.RecordObject(target, "Edit Weight");
                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
