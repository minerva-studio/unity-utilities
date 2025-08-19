using System;
using System.Linq;
using System.Reflection;
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
        public string fullName;
        public string assemblyName;


        public virtual Type Type => typeof(UnityEngine.Object);


        public ScriptReference() { }
        public ScriptReference(Type type)
        {
            SetClass(type);
        }

        public Type GetClass()
        {
            return TryResolve(out var type) ? type : null;
        }

        public void SetClass(Type type)
        {
            if (type == null)
            {
                fullName = string.Empty;
                assemblyName = string.Empty;
            }
            else
            {
                fullName = type.FullName;
                assemblyName = type.Assembly.GetName().Name;
            }
        }

        public bool TryResolve(out Type type)
        {
            type = null;

            var asm = AppDomain.CurrentDomain.GetAssemblies()
                         .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.Ordinal));
            if (asm != null)
                type = asm.GetType(fullName, throwOnError: false);

            if (type == null)
                type = Type.GetType($"{fullName}, {assemblyName}", throwOnError: false);

            if (type == null)
            {
                try
                {
                    var loaded = Assembly.Load(new AssemblyName(assemblyName));
                    type = loaded.GetType(fullName, throwOnError: false);
                }
                catch { }
            }

            return type != null;
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