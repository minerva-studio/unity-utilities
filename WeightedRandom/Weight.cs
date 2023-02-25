using System;

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
    public struct Weight<T> : IWeightable<T>
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
    }
}