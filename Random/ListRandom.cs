using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    public static class ListRandom
    {
        /// <summary>
        /// Random Reorder the list by weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightables"></param>
        public static void RandomReorder<T>(this IList<T> list)
        {
            if (list == null) throw new ArgumentNullException();
            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();
            var randomResult = new List<T>();
            while (list.Count != 0)
            {
                int index = UnityEngine.Random.Range(0, list.Count);
                var value = list[index];
                list.RemoveAt(index);
                randomResult.Add(value);
                list.Remove(value);

            }
            list.Clear();
            foreach (var item in randomResult)
            {
                list.Add(item);
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
        public static T RandomPop<T>(this IList<T> list)
        {
            if (list == null) throw new ArgumentNullException();
            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();


            int index = UnityEngine.Random.Range(0, count);
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
        public static T RandomGet<T>(this IList<T> list)
        {
            if (list == null) throw new ArgumentNullException();


            int count = list.Count;
            if (count == 0) throw new InvalidOperationException();


            int index = UnityEngine.Random.Range(0, count);
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
        public static T RandomGet<T>(this IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException();

            int count = list.Count();
            if (count == 0) throw new InvalidOperationException();


            int index = UnityEngine.Random.Range(0, count);
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
    }
}
