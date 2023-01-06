using System;
using System.Linq;

namespace Minerva.Module
{
    public static class ArrayUtility
    {
        public static T[] Of<T>(params T[] values)
        {
            return values;
        }

        public static T[] Clone<T>(params T[] values) where T : ICloneable
        {
            return values.Select(t => (T)t.Clone()).ToArray();
        }
    }
}