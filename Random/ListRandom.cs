using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    public static class ListRandom
    {
        public delegate int RandomIndexer(int count);

        /// <summary>
        /// Random Reorder the list by weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        public static void RandomReorder<T>(this IList<T> list) => RandomReorder(list, (n) => UnityEngine.Random.Range(0, n));

        /// <summary>
        /// Random Reorder the list by weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        public static void RandomReorder<T>(this IList<T> list, Random random) => RandomReorder(list, (n) => random.Next(n));

        /// <summary>
        /// Random Reorder the list by weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        public static void RandomReorder<T>(this IList<T> list, RandomIndexer indexer)
        {
            if (list == null) throw new ArgumentNullException();
            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();
            //var randomResult = new List<T>();
            for (int i = 0; i < list.Count * 2; i++)
            {
                int index = indexer(list.Count);
                int index2 = indexer(list.Count);
                (list[index2], list[index]) = (list[index], list[index2]);
            }
        }

        /// <summary> 
        /// Pop a random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomPop<T>(this IList<T> list) => RandomPop(list, n => UnityEngine.Random.Range(0, n));

        /// <summary> 
        /// Pop a random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomPop<T>(this IList<T> list, Random random) => RandomPop(list, (n) => random.Next(n));

        /// <summary> 
        /// Pop a random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomPop<T>(this IList<T> list, RandomIndexer indexer)
        {
            if (list == null) throw new ArgumentNullException();
            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();


            int index = indexer(count);
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IList<T> list) => RandomGet(list, n => UnityEngine.Random.Range(0, n));

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IList<T> list, Random random) => RandomGet(list, n => random.Next(n));

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IList<T> list, RandomIndexer indexer)
        {
            if (list == null) throw new ArgumentNullException();


            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();


            int index = indexer(count);
            return list[index];
        }

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IEnumerable<T> list) => RandomGet(list, n => UnityEngine.Random.Range(0, n));

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IEnumerable<T> list, Random random) => RandomGet(list, n => random.Next(n));

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T RandomGet<T>(this IEnumerable<T> list, RandomIndexer indexer)
        {
            if (list == null) throw new ArgumentNullException();

            int count = list.Count();
            if (count == 0) throw new InvalidOperationException();


            int index = indexer(count);
            foreach (var item in list)
            {
                if (index-- <= 0)
                {
                    return item;
                }
            }
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="allowRepeat"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> RandomGet<T>(this IList<T> list, int n, bool allowRepeat = false)
        {
            if (list == null) throw new ArgumentNullException();
            if (n == 0) return new List<T>();
            if (n > list.Count) throw new ArgumentOutOfRangeException();
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
                    result.RemoveAt(UnityEngine.Random.Range(0, result.Count));
                }
                return result;
            }
        }

        /// <summary>
        /// Get a Random element from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="n"></param>
        /// <param name="allowRepeat"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<T> RandomGet<T>(this IEnumerable<T> list, int n, bool allowRepeat = false)
        {
            if (list == null) throw new ArgumentNullException();
            if (n > list.Count()) throw new ArgumentOutOfRangeException();
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

        /// <summary>
        /// An in-position shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this T list) where T : IList
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        /// <summary>
        /// An in-position shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this T list, Random random) where T : IList
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}
