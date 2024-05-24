using System;
using System.Collections;
using System.Collections.Generic;

namespace Minerva.Module
{
    /// <summary>
    /// Array utility same as <see cref="UnityEditor.ArrayUtility"/>
    /// </summary>
    public static class ArrayUtility
    {
        public static void Add<T>(ref T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
        }

        public static bool ArrayEquals<T>(this T[] lhs, T[] rhs)
        {
            if (lhs == null || rhs == null)
            {
                return lhs == rhs;
            }

            if (lhs.Length != rhs.Length)
            {
                return false;
            }

            for (int i = 0; i < lhs.Length; i++)
            {
                if (!lhs[i].Equals(rhs[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ArrayReferenceEquals<T>(this T[] lhs, T[] rhs)
        {
            if (lhs == null || rhs == null)
            {
                return lhs == rhs;
            }

            if (lhs.Length != rhs.Length)
            {
                return false;
            }

            for (int i = 0; i < lhs.Length; i++)
            {
                if ((object)lhs[i] != (object)rhs[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static void AddRange<T>(ref T[] array, T[] items)
        {
            int num = array.Length;
            Array.Resize(ref array, array.Length + items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                array[num + i] = items[i];
            }
        }

        public static void Insert<T>(ref T[] array, int index, T item)
        {
            var newArray = new T[array.Length + 1];
            Array.Copy(array, 0, newArray, 0, index);
            newArray[index] = item;
            Array.Copy(array, index, newArray, index + 1, array.Length - index);
            array = newArray;
        }

        public static void Remove<T>(ref T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Remove(item);
            array = list.ToArray();
        }

        public static T[] FindAll<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindAll(array, match);
        }

        public static T Find<T>(this T[] array, Predicate<T> match)
        {
            return Array.Find(array, match);
        }

        public static int FindIndex<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindIndex(array, match);
        }

        public static int IndexOf<T>(this Array array, T value)
        {
            return Array.IndexOf(array, value);
        }

        public static int LastIndexOf<T>(this T[] array, T value)
        {
            return Array.LastIndexOf(array, value);
        }

        public static void RemoveAt<T>(ref T[] array, int index)
        {
            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            array = list.ToArray();
        }

        public static bool Contains<T>(T[] array, T item)
        {
            List<T> list = new List<T>(array);
            return list.Contains(item);
        }

        public static void Clear<T>(ref T[] array)
        {
            Array.Clear(array, 0, array.Length);
            Array.Resize(ref array, 0);
        }
    }
}