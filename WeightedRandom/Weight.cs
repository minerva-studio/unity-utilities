using System;
using System.Collections.Generic;

namespace Minerva.Module.WeightedRandom
{
    /// <summary>
    /// A default struct for weight
    /// </summary>
    [Serializable]
    public struct Weight : IWeightable
    {
        public object item;
        public int weight;

        public object Item => item;
        int IWeightable.Weight => weight;

        public Weight(object item, int weight)
        {
            this.item = item;
            this.weight = weight;
        }
    }

    /// <summary>
    /// A default struct for weight
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public struct Weight<T> : IWeightable<T>, IEquatable<Weight<T>>
    {
        public T item;
        public int weight;

        object IWeightable.Item => item;
        public T Item => item;
        int IWeightable.Weight => weight;

        public Weight(T item, int weight)
        {
            this.item = item;
            this.weight = weight;
        }

        public override bool Equals(object obj)
        {
            return obj is Weight<T> weight && Equals(weight);
        }

        public readonly bool Equals(Weight<T> weight)
        {
            return EqualityComparer<T>.Default.Equals(item, weight.item)
                && this.weight == weight.weight;
        }

        public static bool operator ==(Weight<T> left, Weight<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Weight<T> left, Weight<T> right)
        {
            return !(left == right);
        }
    }
}