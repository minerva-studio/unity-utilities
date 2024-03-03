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

        public static MonoScript Get<T>() => Get(typeof(T));

        public static MonoScript Get(Type type)
        {
            scripts ??= Init();
            return scripts.TryGetValue(type, out var script) ? script : null;
        }
    }
}