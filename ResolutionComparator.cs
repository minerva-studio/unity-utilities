using System.Collections.Generic;
using UnityEngine;

namespace Minerva.Module
{
    public class ResolutionComparator : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y)
        {
            return x.width == y.width && x.height == y.height;
        }

        public int GetHashCode(Resolution obj)
        {
            return obj.GetHashCode();
        }
    }
}