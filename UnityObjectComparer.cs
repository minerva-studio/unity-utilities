using System.Collections.Generic;

namespace Minerva.Module
{
    /// <summary>
    /// Equality comparator for unity object
    /// </summary>
    public class UnityObjectComparer : IComparer<UnityEngine.Object>
    {
        public int Compare(UnityEngine.Object x, UnityEngine.Object y)
        {
            if (x == y) return 0;
            if (x == null || y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            return x.name.CompareTo(y.name);
        }
    }
}