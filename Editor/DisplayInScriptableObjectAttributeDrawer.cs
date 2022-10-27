using Minerva.Module;
using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(DisplayInSOAttribute))]
    public class DisplayInScriptableObjectAttributeDrawer : PropertyDrawer
    {
        public bool IsLookingAtScriptableObject => Selection.activeObject is ScriptableObject;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DisplayInSOAttribute attr = (DisplayInSOAttribute)attribute;
            return IsLookingAtScriptableObject == attr.displayInScriptableObject ? base.GetPropertyHeight(property, label) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisplayInSOAttribute attr = (DisplayInSOAttribute)attribute;
            //Debug.Log("Draw");
            if (IsLookingAtScriptableObject == attr.displayInScriptableObject)
            {
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