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
        public static T Weight<T>(this List<IWeightable<T>> weightables, int seed) => weightables.WeightNode(seed).Item;
        public static T Weight<T>(this List<IWeightable<T>> weightables) => weightables.WeightNode().Item;
        public static TItem Weight<T, TItem>(this List<T> weightables) where T : IWeightable<TItem> => weightables.WeightNode().Item;



        public static T WeightNode<T>(this List<T> weightables, int seed) where T : IWeightable
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            var result = weightables.WeightNode();
            UnityEngine.Random.state = state;
            return result;
        }

        public static T WeightNode<T>(this List<T> weightables) where T : IWeightable
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




        public static List<T> RandomReorder<T>(this List<IWeightable<T>> weightables) => (List<T>)(object)RandomReorder<IWeightable<T>, T>(weightables);
        public static List<TItem> RandomReorder<T, TItem>(this List<T> weightables) where T : IWeightable<TItem>
        {
            var ret = new List<TItem>();
            var all = new List<T>(weightables);
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
        public static List<T> AllResults<T>(this List<IWeightable<T>> weightables)
        {
            return weightables.Select(w => w.Item).ToList();
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this List<IWeightable<T>> weightables)
        {
            return weightables.Sum(w => w.Weight);
        }

        /// <summary>
        /// get the sum of the weightable list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        /// <returns></returns>
        public static int Sum<T>(this List<T> weightables) where T : IWeightable
        {
            return weightables.Sum(w => w.Weight);
        }
    }

    public static class NonWeightables
    {
        public static T RandomPop<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException();
            }

            int count = list.Count;
            if (count == 0)
            {
                return default;
            }

            int index = UnityEngine.Random.Range(0, count);
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }
        public static T RandomGet<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException();
            }

            int count = list.Count;
            if (count == 0)
            {
                return default;
            }

            int index = UnityEngine.Random.Range(0, count);
            return list[index];
        }

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

        public static List<T> RandomGet<T>(this IList<T> list, int n, bool allowRepeat = false)
        {
            if (n == 0)
            {
                return new List<T>();
            }
            if (n > list.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (n < list.Count / 2)
            {
                List<T> result = new List<T>();
                for (int i = 0; i < n; i++)
                {
                    if (list.Count == 0)
                    {
                        return result;
                    }
                    else
                    {
                        T item = list[UnityEngine.Random.Range(0, list.Count)];
                        if (allowRepeat || !result.Contains(item)) result.Add(item);
                        else i--;
                    }
                }
                return result;
            }
            else
            {
                List<T> result = new List<T>(list);
                for (int i = n; i < list.Count; i++)
                {
                    list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
                }
                return result;
            }
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