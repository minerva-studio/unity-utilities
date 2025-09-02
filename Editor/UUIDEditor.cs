#nullable enable
using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module
{
    [CustomPropertyDrawer(typeof(UUID))]
    public class UUIDEditor : PropertyDrawer
    {
        const float k_ButtonW = 48f;
        const float k_Spacing = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var loProp = property.FindPropertyRelative("lo");
            var hiProp = property.FindPropertyRelative("hi");
            if (loProp is null || hiProp is null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid UUID backing fields");
                return;
            }

            ulong lo = unchecked((ulong)loProp.longValue);
            ulong hi = unchecked((ulong)hiProp.longValue);
            var current = BytesToGuid(lo, hi);

            EditorGUI.BeginProperty(position, label, property);

            var content = EditorGUI.PrefixLabel(position, label);

            // 布局: [TextField] [New]
            var textRect = new Rect(content.x, content.y, content.width - (k_ButtonW + k_Spacing), EditorGUIUtility.singleLineHeight);
            var newRect = new Rect(textRect.xMax + k_Spacing, content.y, k_ButtonW, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            string nextText = EditorGUI.DelayedTextField(textRect, current.ToString());
            if (EditorGUI.EndChangeCheck())
            {
                if (Guid.TryParse(nextText, out var typed))
                {
                    WriteGuid(typed, loProp, hiProp);
                }
                else
                {
                    ShowStatusNotification("Invalid GUID format");
                }
            }

            if (GUI.Button(newRect, "New"))
            {
                WriteGuid(Guid.NewGuid(), loProp, hiProp);
            }

            EditorGUI.EndProperty();
        }

        // ---- Helpers ----
        static void WriteGuid(Guid g, SerializedProperty loProp, SerializedProperty hiProp)
        {
            var bytes = g.ToByteArray();
            ulong lo = BitConverter.ToUInt64(bytes, 0);
            ulong hi = BitConverter.ToUInt64(bytes, 8);

            loProp.longValue = unchecked((long)lo);
            hiProp.longValue = unchecked((long)hi);
            loProp.serializedObject.ApplyModifiedProperties();
        }

        static Guid BytesToGuid(ulong lo, ulong hi)
        {
            Span<byte> b = stackalloc byte[16];
            BitConverter.GetBytes(lo).CopyTo(b);
            BitConverter.GetBytes(hi).CopyTo(b[8..]);
            return new Guid(b);
        }

        static void ShowStatusNotification(string msg)
        {
            if (EditorWindow.focusedWindow != null)
                EditorWindow.focusedWindow.ShowNotification(new GUIContent(msg));
        }
    }
}
