using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Finds custom property drawer for a given type.
    /// From https://forum.unity.com/threads/solved-custompropertydrawer-not-being-using-in-editorgui-propertyfield.534968/
    /// </summary>
    public static class PropertyDrawerFinder
    {
        struct TypeAndFieldInfo
        {
            internal Type type;
            internal FieldInfo fi;
        }

        private static readonly Dictionary<int, TypeAndFieldInfo> s_PathHashVsType = new();
        private static readonly Dictionary<Type, PropertyDrawer> s_TypeVsDrawerCache = new();

        /// <summary>
        /// Searches for custom property drawer for given property, or returns null if no custom property drawer was found.
        /// </summary>
        public static PropertyDrawer FindDrawerForProperty(SerializedProperty property)
        {
            PropertyDrawer drawer;
            TypeAndFieldInfo tfi;

            int pathHash = GetUniquePropertyPathHash(property);

            if (!s_PathHashVsType.TryGetValue(pathHash, out tfi))
            {
                tfi.type = GetPropertyType(property, out tfi.fi);
                s_PathHashVsType[pathHash] = tfi;
            }

            if (tfi.type == null)
                return null;

            if (!s_TypeVsDrawerCache.TryGetValue(tfi.type, out drawer))
            {
                drawer = FindDrawerForType(tfi.type);
                s_TypeVsDrawerCache.Add(tfi.type, drawer);
            }

            if (drawer != null)
            {
                // Drawer created by custom way like this will not have "fieldInfo" field installed
                // It is an optional, but some user code in advanced drawer might use it.
                // To install it, we must use reflection again, the backing field name is "internal FieldInfo m_FieldInfo"
                // See ref file in UnityCsReference (2019) project. Note that name could changed in future update.
                // unitycsreference\Editor\Mono\ScriptAttributeGUI\PropertyDrawer.cs
                var fieldInfoBacking = typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldInfoBacking?.SetValue(drawer, tfi.fi);
            }

            return drawer;
        }

        /// <summary>
        /// Gets type of a serialized property.
        /// </summary>
        static Type GetPropertyType(SerializedProperty property, out FieldInfo fi)
        {
            // To see real property type, must dig into object that hosts it.
            GetPropertyFieldInfo(property, out Type resolvedType, out fi);
            return resolvedType;
        }

        /// <summary>
        /// For caching.
        /// </summary>
        static int GetUniquePropertyPathHash(SerializedProperty property)
        {
            int hash = property.serializedObject.targetObject.GetType().GetHashCode();
            hash += property.propertyPath.GetHashCode();
            return hash;
        }

        static void GetPropertyFieldInfo(SerializedProperty property, out Type resolvedType, out FieldInfo fi)
        {
            fi = property.GetMemberInfo() as FieldInfo;
            resolvedType = fi?.FieldType;
        }

        private static FieldInfo GetFieldInfo(Type parentType, string name)
        {
            FieldInfo fieldInfo = parentType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }
            if (parentType.BaseType != typeof(object) && parentType.BaseType != null)
                return GetFieldInfo(parentType.BaseType, name);
            return null;
        }

        /// <summary>
        /// Returns custom property drawer for type if one could be found, or null if
        /// no custom property drawer could be found. Does not use cached values, so it's resource intensive.
        /// </summary>
        public static PropertyDrawer FindDrawerForType(Type propertyType)
        {
            var cpdType = typeof(CustomPropertyDrawer);
            FieldInfo typeField = cpdType.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo childField = cpdType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

            // Optimization note:
            // For benchmark (on DungeonLooter 0.8.4)
            // - Original, search all assemblies and classes: 250 msec
            // - Wappen optimized, search only specific name assembly and classes: 5 msec
            // - Use type cache now, should be negligible
            foreach (Type candidate in TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>())
            {
                // See if this is a class that has [CustomPropertyDrawer( typeof( T ) )]
                foreach (Attribute a in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                {
                    if (a is not CustomPropertyDrawer drawerAttribute)
                    {
                        continue;
                    }
                    Type drawerType = (Type)typeField.GetValue(drawerAttribute);
                    if (drawerType != propertyType &&
                        (!(bool)childField.GetValue(drawerAttribute) || !propertyType.IsSubclassOf(drawerType)) &&
                        (!(bool)childField.GetValue(drawerAttribute) || !IsGenericSubclass(drawerType, propertyType)))
                    {
                        continue;
                    }
                    if (!candidate.IsSubclassOf(typeof(PropertyDrawer)))
                    {
                        continue;
                    }
                    // Technical note: PropertyDrawer.fieldInfo will not available via this drawer
                    // It has to be manually setup by caller.
                    var drawer = (PropertyDrawer)Activator.CreateInstance(candidate);
                    return drawer;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if the parent type is generic and the child type implements it.
        /// </summary>
        private static bool IsGenericSubclass(Type parent, Type child)
        {
            if (!parent.IsGenericType)
            {
                return false;
            }

            Type currentType = child;
            bool isAccessor = false;
            while (!isAccessor && currentType != null)
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition())
                {
                    isAccessor = true;
                    break;
                }
                currentType = currentType.BaseType;
            }
            return isAccessor;
        }

    }
}