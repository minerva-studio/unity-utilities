using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    [CustomPropertyDrawer(typeof(ScriptReference))]
    [CustomPropertyDrawer(typeof(ScriptReference<>))]
    public class ScriptReferenceDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r = property.GetValue() as ScriptReference;
            var file = r.script;
            var monoScript = (MonoScript)EditorGUI.ObjectField(position, "Type", file, typeof(MonoScript), false);
            if (monoScript && monoScript.GetClass() != null && (monoScript.GetClass().IsSubclassOf(r.Type) || r.Type == monoScript.GetClass()))
            {
                r.script = monoScript;
                r.SetClass(monoScript.GetClass());
            }
            else
            {
                r.script = null;
                r.SetClass(null);
            }
            //Debug.Log(monoScript);
        }
    }
    //public class GenericScriptReferenceDrawer : PropertyDrawer
    //{

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return EditorGUIUtility.singleLineHeight;
    //    }

    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        var r = property.GetValue() as ScriptReference;
    //        var file = r.script;
    //        var monoScript = (MonoScript)EditorGUI.ObjectField(position, "Type", file, typeof(MonoScript), false);
    //        r.script = monoScript;
    //        if (monoScript) r.SetClass(monoScript.GetClass());
    //        else r.SetClass(null);
    //        //Debug.Log(monoScript);
    //    }
    //}
}
