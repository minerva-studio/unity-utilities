using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minerva.Module
{
    public static class Enumerable
    {
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
        /// Create a new list that clones all struct member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> ValueClone<T>(this IEnumerable<T> ts) where T : struct
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
        /// get the most common item in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static T Mode<T>(this IEnumerable<T> ts)
        {
            Dictionary<T, int> keyValuePairs = new();
            foreach (var item in ts)
            {
                if (!keyValuePairs.TryAdd(item, 1))
                    keyValuePairs[item] += 1;
            }
            foreach (var item in keyValuePairs)
                Debug.Log(item.Key + ": " + item.Value);
            return keyValuePairs.OrderByDescending(k => k.Value).FirstOrDefault((a) => true).Key;
        }
    }
}
