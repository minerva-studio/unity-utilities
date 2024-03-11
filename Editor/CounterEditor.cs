using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(Counter))]
    [CustomPropertyDrawer(typeof(Counter<>))]
    public class CounterEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int count = (property.GetValue() as ICollection)?.Count ?? 0;
            return (EditorGUIUtility.singleLineHeight) * (count + 1) + EditorGUIUtility.standardVerticalSpacing * count;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            using (GUIEnable.By(false))
            {
                var singleLine = position;
                singleLine.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.IntField(singleLine, label, (property.GetValue() as ICollection)?.Count ?? 0);
                int index = 0;
                EditorGUI.indentLevel++;
                foreach (var item in property.GetValue() as IEnumerable)
                {
                    singleLine.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    if (item is Object obj)
                    {
                        EditorGUI.ObjectField(singleLine, new GUIContent(index++.ToString()), obj, item?.GetType(), false);
                    }
                    else
                    {
                        EditorGUI.LabelField(singleLine, new GUIContent(index++.ToString()), new GUIContent(item?.GetType().FullName));
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }
    }
}