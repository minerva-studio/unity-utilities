using System;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    public interface IWeightable
    {
        int Weight { get; }
        object Item { get; }

        public int CompareTo(IWeightable weightable)
        {
            return Weight - weightable.Weight;
        }

    }

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
        public static T Weight<T>(this IEnumerable<IWeightable<T>> weightables, int seed) => weightables.WeightNode(seed).Item;
        public static T Weight<T>(this IEnumerable<IWeightable<T>> weightables) => weightables.WeightNode().Item;



        public static T WeightNode<T>(this IEnumerable<T> weightables, int seed) where T : IWeightable
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            var result = weightables.WeightNode();
            UnityEngine.Random.state = state;
            return result;
        }
        public static T WeightNode<T>(this IEnumerable<T> weightables) where T : IWeightable
        {
            if (weightables == null)
            {
                throw new NullReferenceException("The list of weightables is null");
            }
            int totalWeight = weightables.Sum(w => w.Weight);
            if (totalWeight == 0)
            {
                T weightable = weightables.RandomGet();
                return weightable != null ? weightable : default;
            }

            var arr = weightables.ToArray();
            var currentTotal = 0;
            var position = new System.Random().Next(0, totalWeight);
            for (int i = 0; currentTotal < totalWeight && i < arr.Length; i++)
            {
                var weight = arr[i];
                currentTotal += weight.Weight;

                if (currentTotal > position)
                {
                    return weight;
                }
            }
            T weightable2 = weightables.FirstOrDefault();
            return weightable2 != null ? weightable2 : default;
        }




        public static List<T> RandomReorder<T>(this IEnumerable<IWeightable<T>> weightables)
        {
            var ret = new List<T>();
            var all = new List<IWeightable<T>>(weightables);
            for (int i = weightables.Count() - 1; i >= 0; i--)
            {
                var t = all.WeightNode();
                ret.Add(t.Item);
                all.Remove(t);

            }
            return ret;
        }
        /// <summary>
        /// return the enumerable of all possible item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static List<T> AllResults<T>(this IEnumerable<IWeightable<T>> weightables)
        {
            return weightables.Select(w => w.Item).ToList();
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this IEnumerable<IWeightable<T>> weightables)
        {
            return weightables.Sum(w => w.Weight);
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this IEnumerable<T> weightables) where T : IWeightable
        {
            return weightables.Sum(w => w.Weight);
        }
    }

    public static class NonWeightables
    {
        public static T RandomGet<T>(this IEnumerable<T> list)
        {
            int count = list.Count();
            if (count == 0)
            {
                return default;
            }

            int index = UnityEngine.Random.Range(0, count);
            foreach (var item in list)
            {
                if (index-- <= 0)
                {
                    return item;
                }
            }

            return default;
        }

        public static List<T> RandomGet<T>(this IEnumerable<T> list, int n, bool allowRepeat = false)
        {
            var ts = list.ToList();
            List<T> result = new List<T>();
            for (int i = 0; i < n; i++)
            {
                if (ts.Count == 0)
                {
                    return result;
                }
                else
                {
                    T item = ts[UnityEngine.Random.Range(0, ts.Count)];
                    result.Add(item);
                    if (!allowRepeat) ts.Remove(item);
                }
            }
            return result;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> shuffle)
        {
            List<T> list = new List<T>();
            List<T> original = shuffle.ToList();

            for (int i = original.Count() - 1; i >= 0; i--)
            {
                int index = UnityEngine.Random.Range(0, original.Count);
                list.Add(original[index]);
                original.RemoveAt(index);
            }

            return list;
        }
    }
}