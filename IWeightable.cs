using System;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    /// <summary>
    /// Interface for weightable items
    /// </summary>
    public interface IWeightable
    {
        int Weight { get; }
        object Item { get; }

        public int CompareTo(IWeightable weightable)
        {
            return Weight - weightable.Weight;
        }

    }

    /// <summary>
    /// Interface for weightable items
    /// </summary>
    public interface IWeightable<T> : IWeightable
    {
        new T Item { get; }
    }

    /// <summary>
    /// a class handles extension for weightable
    /// </summary>
    public static class Weightable
    {
        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static T Weight<T>(this IList<IWeightable<T>> weightables, int seed) => weightables.WeightNode(seed).Item;

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static T Weight<T>(this IList<IWeightable<T>> weightables) => weightables.WeightNode().Item;

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static TItem Weight<T, TItem>(this IList<T> weightables) where T : IWeightable<TItem> => weightables.WeightNode().Item;



        /// <summary>
        /// Get a weighted node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T WeightNode<T>(this IList<T> weightables, int seed) where T : IWeightable
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            var result = weightables.WeightNode();
            UnityEngine.Random.state = state;
            return result;
        }

        /// <summary>
        /// Get a weighted node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T WeightNode<T>(this IList<T> weightables) where T : IWeightable
        {
            if (weightables == null)
            {
                throw new ArgumentNullException("The list of weightables is null");
            }

            if (weightables.Count == 0)
            {
                return default;
            }

            int totalWeight = weightables.Sum(w => w.Weight);
            if (totalWeight == 0)
            {
                T weightable = weightables.RandomGet();
                return weightable != null ? weightable : default;
            }

            var currentTotal = 0;
            var position = UnityEngine.Random.Range(0, totalWeight);
            T item = default;
            for (int i = 0; currentTotal < totalWeight && i < weightables.Count; i++)
            {
                var weightItem = weightables[i];
                currentTotal += weightItem.Weight;
                item = weightItem;
                if (currentTotal > position)
                {
                    return weightItem;
                }
            }
            return item;
        }



        /// <summary>
        /// Random Reorder the list by weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        public static void RandomReorder<T>(this IList<T> weightables) where T : IWeightable
        {
            var randomResult = new List<T>();
            for (int i = weightables.Count() - 1; i >= 0; i--)
            {
                var t = weightables.WeightNode();
                randomResult.Add(t);
                weightables.Remove(t);

            }
            weightables.Clear();
            foreach (var item in randomResult)
            {
                weightables.Add(item);
            }
        }

        /// <summary>
        /// return the enumerable of all possible item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static List<T> AllResults<T>(this IList<IWeightable<T>> weightables)
        {
            return weightables.Select(w => w.Item).ToList();
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this IList<IWeightable<T>> weightables)
        {
            return weightables.Sum(w => w.Weight);
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this IList<T> weightables) where T : IWeightable
        {
            return weightables.Sum(w => w.Weight);
        }
    }

}