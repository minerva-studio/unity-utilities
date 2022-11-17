using System;
using System.Collections.Generic;

namespace Minerva.Module
{
    /// <summary>
    /// Equality comparator for unity object
    /// </summary>
    public class UnityObjectComparator : IEqualityComparer<UnityEngine.Object>
    {
        public bool Equals(UnityEngine.Object x, UnityEngine.Object y)
        {
            return x == y;
        }

        public int GetHashCode(UnityEngine.Object obj)
        {
            return obj.GetHashCode();
        }
    }
}