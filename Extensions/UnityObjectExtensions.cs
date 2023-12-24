using UnityEngine;

namespace Minerva.Module
{
    public static class UnityObjectExtensions
    {
#nullable enable
        /// <summary>
        /// checking unity object exist, if object does not exist, returns a real null so you can use ?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns>null (real null) if the object does not exist, the object if the object exist</returns>
        public static T? Exist<T>(this T instance) where T : UnityEngine.Object
        {
            return instance ? instance : null;
        }
#nullable disable

        /// <summary>
        /// Get the component, if not exist, then try to add this component to game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (!component) return null;
            var go = component.gameObject;
            if (go.TryGetComponent<T>(out var c)) return c;
            return go.AddComponent<T>();
        }

        /// <summary>
        /// Get the component, if not exist, then try to add this component to game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject component) where T : Component
        {
            if (!component) return null;
            if (component.TryGetComponent<T>(out var c)) return c;
            return component.AddComponent<T>();
        }
    }
}