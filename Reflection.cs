
using System;
using System.Reflection;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Author : Wendi Cai
    /// </summary>
    public static class Reflection
    {
        public static object GetObject(object obj, string path)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            try
            {
                return GetObjectInternal(obj, path.Split('.'));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public static object GetObject(object obj, string[] path)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            try
            {
                return GetObjectInternal(obj, path);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static object GetObjectInternal(object obj, string[] path)
        {
            if (path.Length == 0) return obj;


            var val = GetFieldOrProperty(obj, path[0]);
            if (path.Length == 1)
            {
                return val;
            }
            else
            {
                return GetObjectInternal(val, path[1..]);
            }
        }





        public static object GetLastObject(object obj, string path)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try
            {
                return GetLastObjectInternal(obj, path.Split('.'));
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        private static object GetLastObjectInternal(object obj, string[] path)
        {
            if (path.Length == 0) return obj;


            var val = GetFieldOrProperty(obj, path[0]);
            if (path.Length == 1) return val;

            try
            {
                object v = GetObjectInternal(val, path[1..]);
                return v ?? obj;
            }
            catch (Exception)
            {
                return obj;
            }
        }





        public static object GetFieldOrProperty(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var type = obj.GetType();
            var members = type.GetMember(name);
            foreach (var item in members)
            {
                if (item is FieldInfo field)
                {
                    return field.GetValue(obj);
                }
                else if (item is PropertyInfo property)
                {
                    return property.GetValue(obj);
                }
            }
            return null;
        }
    }
}