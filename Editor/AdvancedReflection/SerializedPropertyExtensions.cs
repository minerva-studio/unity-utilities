using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Provide simple value get/set methods for SerializedProperty.  Can be used with
    /// any data types and with arbitrarily deeply-pathed properties. 
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// (Extension) Get the value of the serialized property.
        public static object GetValue(this SerializedProperty property)
        {
            if (property == null) return null;
            // since unity now introduce this boxed value, try use this method instead of using reflections might be better.
            // however sometimes the property could be an array or something that cannot be accessed by the boxed value
            if (!property.isArray)
            {
                try
                {
                    return property.boxedValue;
                }
                catch { }
            }
            string propertyPath = property.propertyPath;
            object value = property.serializedObject.targetObject;
            int i = 0;
            while (NextPathComponent(propertyPath, ref i, out var token))
                value = GetPathComponentValue(value, token);
            return value;
        }

        /// (Extension) Get the member info of the property if exist
        public static MemberInfo GetMemberInfo(this SerializedProperty property)
        {
            string propertyPath = property.propertyPath;
            object value = property.serializedObject.targetObject;
            object lastValue = property.serializedObject.targetObject;
            PropertyPathComponent token = default;
            PropertyPathComponent lastToken;

            int i = 0;
            while (true)
            {
                lastToken = token;
                if (!NextPathComponent(propertyPath, ref i, out token)) break;

                lastValue = value;
                value = GetPathComponentValue(value, token);
            }

            return GetPathComponentInfo(lastValue, lastToken);
        }

        /// <summary>
        /// (Extension) Set the value of the serialized property.
        /// </summary>
        public static void SetValue(this SerializedProperty property, object value)
        {
            Undo.RecordObject(property.serializedObject.targetObject, $"Set {property.name}");

            property.SetValueNoRecord(value);

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary> 
        /// (Extension) Set the value of the serialized property, but do not record the change.
        /// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
        /// </summary>
        public static void SetValueNoRecord(this SerializedProperty property, object value)
        {
            string propertyPath = property.propertyPath;
            object container = property.serializedObject.targetObject;

            int i = 0;
            NextPathComponent(propertyPath, ref i, out var deferredToken);
            while (NextPathComponent(propertyPath, ref i, out var token))
            {
                container = GetPathComponentValue(container, deferredToken);
                deferredToken = token;
            }
            Debug.Assert(!container.GetType().IsValueType, $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary.  Either change {container.GetType().Name} to a class, or use SetValue with a parent member.");
            SetPathComponentValue(container, deferredToken, value);
        }

        // Union type representing either a property name or array element index.  The element
        // index is valid only if propertyName is null.
        struct PropertyPathComponent
        {
            public string propertyName;
            public int elementIndex;

            public override string ToString()
            {
                return propertyName ?? elementIndex.ToString();
            }
        }

        private const BindingFlags MEMBER_BINDING = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        static Regex arrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

        // Parse the next path component from a SerializedProperty.propertyPath.  For simple field/property access,
        // this is just tokenizing on '.' and returning each field/property name.  Array/list access is via
        // the pseudo-property "Array.data[N]", so this method parses that and returns just the array/list index N.
        //
        // Call this method repeatedly to access all path components.  For example:
        //
        //      string propertyPath = "quests.Array.data[0].goal";
        //      int i = 0;
        //      NextPropertyPathToken(propertyPath, ref i, out var component);
        //          => component = { propertyName = "quests" };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => component = { elementIndex = 0 };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => component = { propertyName = "goal" };
        //      NextPropertyPathToken(propertyPath, ref i, out var component) 
        //          => returns false
        static bool NextPathComponent(string propertyPath, ref int index, out PropertyPathComponent component)
        {
            component = new PropertyPathComponent();

            if (index >= propertyPath.Length)
                return false;

            var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
            if (arrayElementMatch.Success)
            {
                index += arrayElementMatch.Length + 1; // Skip past next '.'
                component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
                return true;
            }

            int dot = propertyPath.IndexOf('.', index);
            if (dot == -1)
            {
                component.propertyName = propertyPath.Substring(index);
                index = propertyPath.Length;
            }
            else
            {
                component.propertyName = propertyPath.Substring(index, dot - index);
                index = dot + 1; // Skip past next '.'
            }

            return true;
        }

        static MemberInfo GetPathComponentInfo(object container, PropertyPathComponent component)
        {
            // cannot get member info from list
            if (component.propertyName == null)
                return null;
            else
                return GetMemberInfo(container, component.propertyName);
        }

        static object GetPathComponentValue(object container, PropertyPathComponent component)
        {
            if (component.propertyName == null)
                return ((IList)container)[component.elementIndex];
            else
                return GetMemberValue(container, component.propertyName);
        }

        static void SetPathComponentValue(object container, PropertyPathComponent component, object value)
        {
            if (component.propertyName == null)
                ((IList)container)[component.elementIndex] = value;
            else
                SetMemberValue(container, component.propertyName, value);
        }

        static MemberInfo GetMemberInfo(object container, string name)
        {
            if (container == null)
                return null;
            var type = container.GetType();
            return GetInfo(type, name);
        }

        static object GetMemberValue(object container, string name)
        {
            if (container == null)
                return null;
            var type = container.GetType();
            var member = GetInfo(type, name);

            if (member == null)
            {
                //Debug.LogFormat("Error when looking for member {0}", name);
                return null;
            }
            if (Attribute.IsDefined(member, typeof(ObsoleteAttribute))) return null;
            if (member is FieldInfo field) return field.GetValue(container);
            if (member is PropertyInfo property) return property.GetValue(container);
            return null;
        }

        static void SetMemberValue(object container, string name, object value)
        {
            var type = container.GetType();
            var member = GetInfo(type, name);//, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                                             //var members = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (member is FieldInfo field)
            {
                field.SetValue(container, value);
                return;
            }
            if (member is PropertyInfo property)
            {
                property.SetValue(container, value);
                return;
            }
            Debug.Assert(false, $"Failed to set member {container}.{name} via reflection");
        }

        static MemberInfo GetInfo(Type parentType, string name)
        {
            FieldInfo fieldInfo = parentType.GetField(name, MEMBER_BINDING);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }
            PropertyInfo propertyInfo = parentType.GetProperty(name, MEMBER_BINDING);
            if (propertyInfo != null)
            {
                return propertyInfo;
            }
            if (parentType.BaseType != typeof(object) && parentType.BaseType != null)
                return GetInfo(parentType.BaseType, name);
            return null;
        }
    }
}