using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Comparing game resolution
    /// </summary>
    public class ResolutionComparator : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y)
        {
            return x.width == y.width && x.height == y.height;
        }

        public int GetHashCode(Resolution obj)
        {
            return HashCode.Combine(obj.width, obj.height);
        }
    }
}