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
                bool matches = value is null || attr.EqualsAny(value);
                return GetFieldHeight(property, label, matches);
            }
            catch
            {
                return GetBasePropertyHeight(property, label);
            };
        }

        protected float GetBasePropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
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
                if (value is null)
                {
                    label.text += " (default mode)";
                    label.tooltip += $"Cannot found the path {attr.path} or it is not a boolean";
                    if (value is null) Debug.LogWarning("value found is " + value);
                    return;
                }
                DrawField(position, property, label, attr.EqualsAny(value) == attr.result);
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
                //ScriptAttributeUtility
                //PropertyHandler
                var type = Type.GetType("ScriptAttributeUtility");
                var method = type.GetMethod("GetHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                object handler = method.Invoke(null, new object[] { property });
                var OnGUIMethod = handler.GetType().GetMethod("OnGUI", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                OnGUIMethod.Invoke(handler, new object[] { position, property, null, true });
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