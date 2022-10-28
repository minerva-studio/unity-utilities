using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [Obsolete]
    [CustomPropertyDrawer(typeof(CubeInt))]
    public class CubeIntEditor : PropertyDrawer
    {
        private const int PropertyHeight = 20;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CubeInt range;
            position.height = PropertyHeight - 2;
            label = EditorGUI.BeginProperty(position, label, property);
            //            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.LabelField(position, label);

            //EditorGUILayout.Space(2);
            SerializedProperty XminP = property.FindPropertyRelative("m_Xmin");
            SerializedProperty YminP = property.FindPropertyRelative("m_Ymin");
            SerializedProperty ZminP = property.FindPropertyRelative("m_Zmin");
            var minX = XminP.intValue;
            var minY = YminP.intValue;
            var minZ = ZminP.intValue;
            var v3 = new Vector3Int(minX, minY, minZ);

            label.text = "Position";
            EditorGUI.indentLevel++;
            position.y += PropertyHeight;
            var newV3 = EditorGUI.Vector3IntField(position, label, v3);

            XminP.intValue = newV3.x;
            YminP.intValue = newV3.y;
            ZminP.intValue = newV3.z;


            SerializedProperty XsizeP = property.FindPropertyRelative("m_Width");
            SerializedProperty YsizeP = property.FindPropertyRelative("m_Height");
            SerializedProperty ZsizeP = property.FindPropertyRelative("m_Depth");
            var sizeX = XsizeP.intValue;
            var sizeY = YsizeP.intValue;
            var sizeZ = ZsizeP.intValue;
            v3 = new Vector3Int(sizeX, sizeY, sizeZ);
            position.y += PropertyHeight;
            //EditorGUILayout.Space(2);
            label.text = "Size";
            newV3 = EditorGUI.Vector3IntField(position, label, v3);
            XminP.intValue = newV3.x;
            YminP.intValue = newV3.y;
            ZminP.intValue = newV3.z;




            EditorGUI.indentLevel--;
            //EditorGUILayout.EndHorizontal();
            EditorGUI.EndProperty();
        }

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    EditorGUI.PropertyField(position, property, label);
        //}

        //public override VisualElement CreatePropertyGUI(SerializedProperty property)
        //{
        //    // Create property container element.
        //    var container = new VisualElement();

        //    // Create property fields.
        //    var max = new PropertyField(property.FindPropertyRelative("max"));
        //    var min = new PropertyField(property.FindPropertyRelative("min"));

        //    // Add fields to the container.
        //    container.Add(max);
        //    container.Add(min);

        //    return container;
        //}
    }
}
