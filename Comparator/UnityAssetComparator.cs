using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Equality comparator for Prefabs
    /// <br/>
    /// Note: Prefab reference might point to a component, not the gameobject itself. This can cause some duplication on a list of prefabs
    /// </summary>
    public class UnityAssetComparator : IEqualityComparer<Object>
    {
        public bool Equals(Object x, Object y)
        {
            if (x is ScriptableObject && y is ScriptableObject)
            {
                return x == y;
            }
            var gx = x is Component cx ? cx.gameObject : (x is GameObject xg ? xg : x);
            var gy = x is Component cy ? cy.gameObject : (y is GameObject yg ? yg : y);

            return gx == gy;
        }

        public int GetHashCode(Object obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Equality comparator for Prefabs
    /// <br/>
    /// Note: Prefab reference might point to a component, not the gameobject itself. This can cause some duplication on a list of prefabs
    /// </summary>
    public class UnityAssetComparator<T> : IEqualityComparer<T> where T : Object
    {
        public bool Equals(T x, T y)
        {
            if (x is ScriptableObject && y is ScriptableObject)
            {
                return x == y;
            }
            Object gx = x is Component cx ? cx.gameObject : (x is GameObject xg ? xg : x);
            Object gy = x is Component cy ? cy.gameObject : (y is GameObject yg ? yg : y);

            return gx == gy;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Equality comparator for Prefabs
    /// <br/>
    /// Note: Prefab reference might point to a component, not the gameobject itself. This can cause some duplication on a list of prefabs
    /// </summary>
    public class UnityAssetNameComparator<T> : IEqualityComparer<T> where T : Object
    {
        public bool Equals(T x, T y)
        {
            return x.name == y.name;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}