#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
#endif


namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(EnumStringDropdownAttribute))]
    public class EnumStringDropdownAttributeDrawer : PropertyDrawer
    {
        private const string reset = "Reset to normal text bar";

        public string[] ListToStringArray(IList list)
        {
            string[] result = new string[list.Count + 1];
            for (int i = 0; i < list.Count; i++)
            {
                object obj = list[i];
                result[i] = obj.ToString();
            }
            result[list.Count] = reset;
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                EnumStringDropdownAttribute dropdownAttribute = (EnumStringDropdownAttribute)attribute;

                Type itemType = dropdownAttribute.Type;//store the type  
                var listObj = Enum.GetValues(itemType) as IList;//get the list from path

                //Debug.Log($"the object: {JsonUtility.ToJson(listObj)}"); 
                if (listObj.Count > 0)
                {
                    #region Draw the list dropdown

                    object obj = property.GetValue();
                    string[] arr = ListToStringArray(listObj);

                    var contents = arr.Select(
                        selector).ToArray();
                    int SelectedID = 0;
                    try
                    {
                        SelectedID = (int)Enum.Parse(itemType, obj.ToString());
                    }
                    catch (Exception)
                    {
                        EditorGUI.PropertyField(position, property, label);
                        return;
                    }


                    int newSelectedID = EditorGUI.Popup(position, label, SelectedID, contents);
                    if (newSelectedID != SelectedID)
                    {//changed
                        if (newSelectedID >= listObj.Count)
                        {
                            property.SetValue("");
                            EditorGUI.PropertyField(position, property, label);
                            return;
                        }
                        SelectedID = newSelectedID;
                        object selectedObject = listObj[SelectedID].ToString();
                        property.SetValue(selectedObject);//property.setvalue to the serialized object

                        EditorUtility.SetDirty(property.serializedObject.targetObject);//repaint 
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                //GUIStyle style = GetGUIColor(Color.red);
                //style.wordWrap = true;//word wrap

                //GUILayout.BeginHorizontal();

                //GUILayout.Label(property.name + " [Dropdown ERROR] ");
                //GUILayout.TextArea(e.ToString(), style, GUILayout.ExpandHeight(true));

                //GUILayout.EndHorizontal();
                EditorGUI.PropertyField(position, property, label);
                Debug.LogException(e);
            }

        }


        static GUIContent selector(string s)
        {
            string text = string.Concat(s.Select(x => (Char.IsUpper(x) ? " " + x : x.ToString()))).TrimStart(' ');
            text = text[0].ToString().ToUpper()[0] + text.Substring(1, text.Length - 1);
            return new GUIContent(text);
        }

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    try
        //    {
        //        EnumStringDropdownAttribute dropdownAttribute = (EnumStringDropdownAttribute)attribute;

        //        Type itemType = dropdownAttribute.Type;//store the type  
        //        var listObj = Enum.GetValues(itemType) as IList;//get the list from path

        //        //Debug.Log($"the object: {JsonUtility.ToJson(listObj)}"); 
        //        if (listObj.Count > 0)
        //        {
        //            #region Draw the list dropdown

        //            object obj = property.GetValue();
        //            string[] arr = ListToStringArray(listObj);
        //            int SelectedID = 0;
        //            try
        //            {
        //                SelectedID = (int)Enum.Parse(itemType, obj.ToString());
        //            }
        //            catch (Exception)
        //            {
        //                EditorGUI.PropertyField(position, property, label);
        //                return;
        //            }


        //            int newSelectedID = EditorGUI.Popup(position, property.name, SelectedID, arr, GetDropdownStyle());
        //            if (newSelectedID != SelectedID)
        //            {//changed
        //                if (newSelectedID >= listObj.Count)
        //                {
        //                    property.SetValue("");
        //                    EditorGUI.PropertyField(position, property, label);
        //                    return;
        //                }
        //                SelectedID = newSelectedID;
        //                object selectedObject = listObj[SelectedID].ToString();
        //                property.SetValue(selectedObject);//property.setvalue to the serialized object

        //                EditorUtility.SetDirty(property.serializedObject.targetObject);//repaint 
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //GUIStyle style = GetGUIColor(Color.red);
        //        //style.wordWrap = true;//word wrap

        //        //GUILayout.BeginHorizontal();

        //        //GUILayout.Label(property.name + " [Dropdown ERROR] ");
        //        //GUILayout.TextArea(e.ToString(), style, GUILayout.ExpandHeight(true));

        //        //GUILayout.EndHorizontal();
        //        EditorGUI.PropertyField(position, property, label);
        //        Debug.LogException(e);
        //    }

        //}
    }
}
