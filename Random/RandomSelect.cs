using System;

namespace Minerva.Module
{
    /// <summary>
    /// Random select an element
    /// </summary>
    public static class RandomSelect
    {
        public static T From<T>(params T[] values)
        {
            var i = UnityEngine.Random.Range(0, values.Length);
            return values[i];
        }

        public static T From<T>(this Random random, params T[] values)
        {
            var i = random.Next(values.Length);
            return values[i];
        }

        public static T From<T>(params (T, int weight)[] values)
        {
            return WeightedRandom.Weightable.Weight(values, e => e.Item1, e => e.weight);
        }

        public static T From<T>(this Random random, params (T, int weight)[] values)
        {
            return WeightedRandom.Weightable.Weight(values, e => e.Item1, e => e.weight, n => random.Next(n));
        }
    }
}
