using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A cache allowing getting mono script of an class (if exist)
    /// </summary>
    public class MonoScriptCache
    {
#if UNITY_EDITOR
        static Dictionary<Type, MonoScript> scripts;
        static Dictionary<Type, MonoScript> Init()
        {
            scripts = new Dictionary<Type, UnityEditor.MonoScript>();
            foreach (var item in Resources.FindObjectsOfTypeAll<UnityEditor.MonoScript>())
            {
                Type type = item.GetClass();
                if (type != null)
                {
                    scripts[type] = item;
                }
            }
            return scripts;
        }

#endif

        public static MonoScript Get<T>() => Get(typeof(T));

        public static MonoScript Get(Type type)
        {
#if UNITY_EDITOR
            scripts ??= Init();
            return scripts.TryGetValue(type, out var script) ? script : null;
#else   
            return null;
#endif
        }
    }
}