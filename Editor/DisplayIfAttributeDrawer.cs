using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{

    [CustomPropertyDrawer(typeof(DisplayIfAttribute))]
    public class DisplayIfAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            try
            {
                DisplayIfAttribute attr = (DisplayIfAttribute)attribute;
                object value = GetValue(property, attr);

                //something not match
                if (value is null)
                {
                    return base.GetPropertyHeight(property, label);
                }
                if (DisplayIfAttribute.EqualsAny(value, attr.expectValues))
                {
                    return base.GetPropertyHeight(property, label);
                }
                return 0;
            }
            catch { }
            return 0;
        }

        private object GetValue(SerializedProperty property, DisplayIfAttribute attr)
        {
            string propertyPath = property.propertyPath;
            string path = propertyPath.Contains(".") ? propertyPath[..(propertyPath.LastIndexOf('.') + 1)] : "";
            path += attr.name;
            SerializedProperty targetProperty = property.serializedObject.FindProperty(path);
            if (null == targetProperty)
            {
                return null;
            }
            return targetProperty.GetValue();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisplayIfAttribute attr = (DisplayIfAttribute)attribute;
            //Debug.Log("Draw");
            try
            {
                object value = GetValue(property, attr);

                if (value is null)
                {
                    label.text += " (default mode)";
                    label.tooltip += $"Cannot found the path {attr.name} or it is not a boolean";
                    if (value is null) Debug.LogWarning("value found is " + value);
                    //EditorGUI.PropertyField(position, property, label, true);
                    return;
                }
                if (DisplayIfAttribute.EqualsAny(value, attr.expectValues))
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                label.text += " (default mode)";
                label.tooltip += $"Cannot found the path {attr.name} or it is not a boolean";
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}