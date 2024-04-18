using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(DisplayIfAttribute))]
    public class DisplayIfAttributeDrawer : ConditionalFieldAttributeDrawer
    {
        protected override float GetFieldHeight(SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            if (conditionMatches)
            {
                return GetBasePropertyHeight(property, label);
            }
            else return 0;// -EditorGUIUtility.standardVerticalSpacing; // if the property is hinding, return the negative of standard vertical spacing
        }

        protected override void DrawField(Rect position, SerializedProperty property, GUIContent label, bool conditionMatches)
        {
            property.serializedObject.Update();
            if (conditionMatches) DrawDefault(position, property, label);
        }
    }
}