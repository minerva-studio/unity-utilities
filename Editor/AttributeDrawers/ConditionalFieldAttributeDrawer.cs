using System;
using UnityEditor;
using UnityEngine;


namespace Minerva.Module.Editor
{
    public abstract class ConditionalFieldAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            try
            {
                ConditionalFieldAttribute attr = (ConditionalFieldAttribute)attribute;
                object value = GetValue(property, attr);

                //something not match
                bool matches = value == null || (attr.Matches(value));
                return GetFieldHeight(property, label, matches);
            }
            catch (ExitGUIException) { throw; }
            catch (Exception e)
            {
                Debug.LogException(e);
                return GetBasePropertyHeight(property, label);
            };
        }

        protected static float GetBasePropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorFieldDrawers.GetPropertyHeight(property, label);
        }

        protected static void DrawDefault(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorFieldDrawers.PropertyField(position, property, label, true);
        }

        private object GetValue(SerializedProperty property, ConditionalFieldAttribute attr)
        {
            string propertyPath = property.propertyPath;
            string path = propertyPath.Contains(".") ? propertyPath[..(propertyPath.LastIndexOf('.') + 1)] : "";
            path += attr.path;
            SerializedProperty targetProperty = property.serializedObject.FindProperty(path);
            if (null == targetProperty)
            {
                return null;
            }
            return targetProperty.GetValue();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalFieldAttribute attr = (ConditionalFieldAttribute)attribute;
            //Debug.Log("Draw");
            try
            {
                object value = GetValue(property, attr);
                //if (value is null)
                //{
                //    label.text += " (default mode)";
                //    label.tooltip += $"Cannot found the path {attr.path} or it is not a boolean";
                //    if (value is null) Debug.LogWarning("value found is " + value);
                //    return;
                //}
                //something not match
                bool matches = attr.Matches(value);
                DrawField(position, property, label, matches);
            }
            catch (ExitGUIException)
            {
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                DrawFailed(position, property, label, attr);
            }
        }

        /// <summary>
        /// Calling <see cref="ScriptAttributeUtility"/> and <see cref="PropertyHandler"/> by using reflection
        /// </summary>
        private static void DrawFailed(Rect position, SerializedProperty property, GUIContent label, ConditionalFieldAttribute attr)
        {
            label.text += " (default mode)";
            label.tooltip += $"Cannot found the path {attr.path} or it is not a boolean";
            try
            {
                DrawDefault(position, property, label);
            }
            catch (ExitGUIException) { throw; }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorGUI.PropertyField(position, property, label, true);
            }

        }

        protected abstract float GetFieldHeight(SerializedProperty property, GUIContent label, bool conditionMatches);
        protected abstract void DrawField(Rect position, SerializedProperty property, GUIContent label, bool conditionMatches);
    }
}