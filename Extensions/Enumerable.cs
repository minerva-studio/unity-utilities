using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minerva.Module
{
    public static class Enumerable
    {
        /// <summary>
        /// return a list with items of the same reference but in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static IEnumerable<T> ShallowClone<T>(this IEnumerable<T> ts)
        {
            if (ts is null) throw new ArgumentNullException();

            T[] clone = ts.ToArray();
            return clone;
        }
        /// <summary>
        /// return a list with items of the same reference but in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> ShallowCloneToList<T>(this IEnumerable<T> ts)
        {
            if (ts is null) throw new ArgumentNullException();

            List<T> newList = new List<T>(ts);
            return newList;
        }

        /// <summary>
        /// return a list with a cloned items and in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> DeepClone<T>(this IEnumerable<T> list) where T : ICloneable
        {
            if (list is null) throw new ArgumentNullException();

            T[] clone = list.Select(s => (T)s.Clone()).ToArray();
            return clone;
        }
        /// <summary>
        /// return a list with a cloned items and in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> DeepCloneToList<T>(this IEnumerable<T> list) where T : ICloneable
        {
            if (list is null) throw new ArgumentNullException();

            List<T> newList = new();
            foreach (T item in list)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }

        /// <summary>
        /// find whether two IEnumerable matchs all elements in the list (same count, call IEquatable)
        /// <br></br>
        /// as list == list for all element (unordered)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool MatchAll<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            if (ts1.Count() != ts2.Count()) return false;
            return ts1.ContainsAll(ts2);
        }

        /// <summary>
        /// find whether two IEnumerable matchs all elements in the list (same count, call IEquatable)
        /// <br></br>
        /// as list == list for all element (ordered)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool IndenticalTo<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            if (ts1.Count() != ts2.Count()) return false;
            return ts1.ContainsAll(ts2);
        }

        /// <summary>
        /// find whether first IEnumerable contains all elements in the second list (all IEquatable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            return ts2.All(t => ts1.Contains(t));
        }

        /// <summary>
        /// find whether first IEnumerable contains any elements in the second list (all IEquatable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            return ts2.Any(t => ts1.Contains(t));
        }

        /// <summary>
        /// get the most common item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static T Mode<T>(this IEnumerable<T> ts)
        {
            Dictionary<T, int> keyValuePairs = new Dictionary<T, int>();
            foreach (var item in ts)
            {
                if (!keyValuePairs.ContainsKey(item))
                    keyValuePairs.Add(item, 1);
                else
                    keyValuePairs[item] += 1;
            }
            foreach (var item in keyValuePairs)
                Debug.Log(item.Key + ": " + item.Value);
            return keyValuePairs.OrderByDescending(k => k.Value).FirstOrDefault((a) => true).Key;
        }

        /// <summary>
        /// Create a new list that clones all struct member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> StructClone<T>(this IEnumerable<T> ts) where T : struct
        {
            if (ts is null)
            {
                return null;
            }
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add(item);
            }
            return newList;
        }

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
                    list.RemoveAt(UnityEngine.Random.Range(0, list.Count));
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
        /// Return a shuffled Enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="shuffle"></param>
        /// <returns></returns>
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
