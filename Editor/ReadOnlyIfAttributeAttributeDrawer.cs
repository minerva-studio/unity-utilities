using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyIfAttribute))]
    public class ReadOnlyIfAttributeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyIfAttribute attr = (ReadOnlyIfAttribute)attribute;
            //Debug.Log("Draw");
            try
            {
                object BaseMaster = property.serializedObject.targetObject;//store the base master
                object value = ReflectionSystem.GetValue(BaseMaster, attr.listPath);//get the list from path

                if (value is null || value.GetType() != attr.expectValue.GetType())
                {
                    if (attr.allowPathNotFoundEdit)
                    {
                        EditorGUI.PropertyField(position, property, label);
                    }
                    else
                    {
                        label.text += " (default mode)";
                        label.tooltip += $"Cannot found the path {attr.listPath} or it is not a boolean";
                        if (value is null) Debug.LogError("value found is " + value);
                        EditorGUI.PropertyField(position, property, label);
                    }
                    return;
                }
                else if (MatchWithExpect(attr.expectValue, value))
                {
                    GUI.enabled = false;
                    EditorGUI.PropertyField(position, property, label);
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                }//Debug.Log(value);if (!(value is bool))

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                label.text += " (default mode)";
                label.tooltip += $"Cannot found the path {attr.listPath} or it is not a boolean";
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private static bool MatchWithExpect(object expect, object value)
        {
            if (expect is IComparable)
                return value is IComparable b && b.CompareTo(expect) == 0;
            return value.Equals(expect);
        }

    }
}