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
            var scriptProperty = property.FindPropertyRelative(nameof(ScriptReference.script));
            EditorGUI.PropertyField(position, scriptProperty);

            //Debug.Log(raw);
            //Debug.Log(file != null);
            //Debug.Log(monoScript != null);
            //if (monoScript && monoScript.GetClass() != null)
            //{
            //    Debug.Log(monoScript.GetType());
            //    Debug.Log(monoScript.name);
            //    Debug.Log(monoScript.GetClass().Name);
            //}
            SerializedProperty aname = property.FindPropertyRelative(nameof(ScriptReference.assemblyName));
            SerializedProperty cname = property.FindPropertyRelative(nameof(ScriptReference.fullName));
            var monoScript = scriptProperty.boxedValue as MonoScript;
            System.Type type = monoScript ? monoScript.GetClass() : null;
            if (monoScript && monoScript.GetClass() != null
                && (monoScript.GetClass().IsSubclassOf(r.Type) || r.Type == monoScript.GetClass())
                )
            {
                aname.stringValue = type.Assembly.GetName().Name;
                cname.stringValue = type.FullName;
            }
            else
            {
                aname.stringValue = string.Empty;
                cname.stringValue = string.Empty;
            }
            property.serializedObject.ApplyModifiedProperties();
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
