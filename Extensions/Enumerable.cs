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

            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add(item);
            }
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
        public static List<T> StructClone<T>(this List<T> ts) where T : struct
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
    }
}
