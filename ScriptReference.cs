using System;
namespace Minerva.Module
{
    /// <summary>
    /// class that refer to an custom script, usable outside editor
    /// </summary>
    [Serializable]
    public class ScriptReference
    {
#if UNITY_EDITOR
        public UnityEditor.MonoScript script;
#endif
        public string assemblyQualifiedName = "";


        public virtual Type Type => typeof(UnityEngine.Object);


        public ScriptReference() { }
        public ScriptReference(Type type)
        {
            SetClass(type);
        }

        public Type GetClass()
        {
            return Type.GetType(assemblyQualifiedName);
        }

        public void SetClass(Type type)
        {
            if (type == null)
            {
                assemblyQualifiedName = string.Empty;
            }
            else
            {
                assemblyQualifiedName = type.AssemblyQualifiedName;
            }
        }

        public static implicit operator Type(ScriptReference cr)
        {
            return cr.GetClass();
        }

#if UNITY_EDITOR 
        public ScriptReference(UnityEditor.MonoScript monoScript)
        {
            if (!monoScript)
            {
                return;
            }
            script = monoScript;
            SetClass(monoScript.GetClass());
        }

        public static implicit operator ScriptReference(UnityEditor.MonoScript cr)
        {
            return !cr ? new ScriptReference() : new ScriptReference(cr);
        }
#endif
    }

    /// <summary>
    /// class that refer to an custom script
    /// </summary>
    [Serializable]
    public class ScriptReference<T> : ScriptReference
    {
        public override Type Type => typeof(T);

        public ScriptReference() : base() { }
        public ScriptReference(Type type) : base(type) { }

#if UNITY_EDITOR 
        public ScriptReference(UnityEditor.MonoScript type) : base(type)
        {
        }

        public static implicit operator ScriptReference<T>(UnityEditor.MonoScript cr)
        {
            return !cr ? new ScriptReference<T>() : new ScriptReference<T>(cr);
        }

#endif
    }
}