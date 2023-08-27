using Minerva.Module.WeightedRandom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module.WeightedRandom
{
    /// <summary>
    /// a class handles extension for weightable
    /// </summary>
    public static class Weightable
    {

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <typeparam name="TItem">Item type</typeparam>
        /// <param name="weightables"></param>
        /// <param name="items"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static TItem Weight<T, TItem>(this IList<T> weightables, Func<T, (TItem, int)> items)
            => Weight(weightables, i => items(i).Item1, i => items(i).Item2);

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <typeparam name="TItem">Item type</typeparam>
        /// <param name="weightables"></param>
        /// <param name="items"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static TItem Weight<T, TItem>(this IList<T> weightables, Func<T, TItem> items, Func<T, int> weight)
        {
            if (weightables == null) throw new ArgumentNullException("The list of weightables is null");


            if (weightables.Count == 0) return default;

            int totalWeight = weightables.Sum(w => weight(w));
            if (totalWeight == 0)
            {
                T weightable = weightables.RandomGet();
                return weightable != null ? items(weightable) : default;
            }

            int currentTotal = 0;
            int position = UnityEngine.Random.Range(0, totalWeight);
            T item = default;
            for (int i = 0; currentTotal < totalWeight && i < weightables.Count; i++)
            {
                var weightItem = weightables[i];
                currentTotal += weight(weightItem);
                item = weightItem;
                if (currentTotal > position)
                {
                    return items(weightItem);
                }
            }
            return items(item);
        }

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static T Weight<T>(this IList<IWeightable<T>> weightables, UnityEngine.Random.State seed) => weightables.WeightNode(seed).Item;

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
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static TItem Weight<TItem>(this IList<Weight<TItem>> weightables) => weightables.WeightNode().Item;

        /// <summary>
        /// return a random item from the list, distributed by the weight given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns> 
        public static object Weight(this IList<Weight> weightables) => weightables.WeightNode().Item;



        /// <summary>
        /// Get a weighted node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T WeightNode<T>(this IList<T> weightables, UnityEngine.Random.State seed) where T : IWeightable
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.state = seed;
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
            return WeightNode(weightables, WeightSum(weightables));
        }

        /// <summary>
        /// Get a weighted node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T PopWeightNode<T>(this IList<T> weightables) where T : IWeightable
        {
            return PopWeightNode(weightables, WeightSum(weightables));
        }

        internal static T WeightNode<T>(this IList<T> weightables, int sum) where T : IWeightable
        {
            if (sum == 0)
            {
                if (weightables.Count == 0) return default;
                T weightable = weightables[UnityEngine.Random.Range(0, weightables.Count)];
                return weightable;
            }

            int currentTotal = 0;
            int position = UnityEngine.Random.Range(0, sum);
            for (int i = 0; currentTotal < sum && i < weightables.Count; i++)
            {
                currentTotal += weightables[i].Weight;
                if (currentTotal > position) return weightables[i];
            }
            // last item
            return weightables[^1];
        }

        internal static T PopWeightNode<T>(this IList<T> weightables, int sum) where T : IWeightable
        {
            if (weightables.Count == 0) return default;
            if (sum == 0)
            {
                T weightable = weightables[UnityEngine.Random.Range(0, weightables.Count)];
                return weightable;
            }

            int currentTotal = 0;
            int position = UnityEngine.Random.Range(0, sum);
            for (int i = 0; currentTotal < sum && i < weightables.Count; i++)
            {
                currentTotal += weightables[i].Weight;
                if (currentTotal > position)
                {
                    T t = weightables[i];
                    weightables.RemoveAt(i);
                    return t;
                }
            }
            T t1 = weightables[^1];
            weightables.RemoveAt(weightables.Count - 1);
            // last item
            return t1;
        }

        public static int WeightSum<T>(this IList<T> weightables) where T : IWeightable
        {
            int sum = 0;
            for (int i = 0; i < weightables.Count; i++)
            {
                sum += weightables[i].Weight;
            }
            return sum;
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int WeightSum<T>(this IList<IWeightable<T>> weightables)
        {
            int sum = 0;
            for (int i = 0; i < weightables.Count; i++)
            {
                sum += weightables[i].Weight;
            }
            return sum;
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
    }
}