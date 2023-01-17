using UnityEngine;

namespace Minerva.Module
{
    public static class UnityObjectExtensions
    {
        /// <summary>
        /// checking unity object exist, if object does not exist, returns a real null so you can use ?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns>null (real null) if the object does not exist, the object if the object exist</returns>
        public static T Exist<T>(this T instance) where T : UnityEngine.Object
        {
            return instance ? instance : null;
        }

        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject component) where T : Component
        {
            T t = component.GetComponent<T>();
            if (t) { return t; }
            return component.gameObject.AddComponent<T>();
        }
    }
}