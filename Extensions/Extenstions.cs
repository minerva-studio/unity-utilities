using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Minerva.Module
{

    public static class Extensions
    {

        public static List<T> Clone<T>(this List<T> ts) where T : ICloneable
        {
            if (ts is null)
            {
                return null;
            }
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }

        /// <summary>
        /// return a list with items of the same reference but in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> ShallowClone<T>(this IEnumerable<T> ts)
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
        /// return a list with a cloned items and in a different list instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static List<T> DeepClone<T>(this IEnumerable<T> ts) where T : ICloneable
        {
            if (ts is null) return null;

            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }


        public static IEnumerable<T> Clone<T>(this IEnumerable<T> ts) where T : ICloneable
        {
            List<T> newList = new List<T>();
            foreach (T item in ts)
            {
                newList.Add((T)item.Clone());
            }
            return newList;
        }

        /// <summary>
        /// find whether two IEnumerable matchs all elements in the list (same count, call IEquatable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool MatchAll<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            if (ts1.Count() != ts2.Count()) return false;
            return ts1.Except(ts2).Count() == 0;
        }

        /// <summary>
        /// find whether fisrt IEnumerable contains all elements in the second list (all IEquatable)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts1"></param>
        /// <param name="ts2"></param>
        /// <returns></returns>
        public static bool ContainAll<T>(this IEnumerable<T> ts1, IEnumerable<T> ts2)
        {
            return ts2.Except(ts1).Count() == 0;
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
                if (!keyValuePairs.ContainsKey(item)) keyValuePairs.Add(item, 1);
                else keyValuePairs[item] += 1;
            }
            foreach (var item in keyValuePairs) Debug.Log(item.Key + ": " + item.Value);
            return keyValuePairs.OrderByDescending(k => k.Value).FirstOrDefault((a) => true).Key;
        } 

    }

    public static class Extension2
    {
        public static List<T> Clone<T>(this List<T> ts) where T : struct
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