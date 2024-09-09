using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Minerva.Module
{

    /// <summary>
    /// A semaphore like structure for the game to count something.
    /// </summary>
    [Serializable]
    public class Counter : ICounter<UnityObject>
    {
        private List<UnityObject> counter;
        public event Action<int> OnFilled;

        List<UnityObject> ICounter<UnityObject>.Counter => counter;
        public int Count => counter.Count;

        public Counter()
        {
            counter = new List<UnityObject>();
        }

        public Counter(Action<int> baseAction) : this()
        {
            OnFilled = baseAction;
        }

        public bool Add(UnityObject obj)
        {
            // not a valid object
            if (!obj) return false;
            RemoveInvalid();
            if (counter.Contains(obj))
            {
                return false;
            }
            counter.Add(obj);
            OnFilled?.Invoke(counter.Count);
            return true;
        }

        public bool Remove(UnityObject obj)
        {
            if (!obj) { return false; }
            RemoveInvalid();
            bool v = counter.Remove(obj);
            if (v) OnFilled?.Invoke(counter.Count);
            return v;
        }

        private void RemoveInvalid()
        {
            counter.RemoveAll(o => !o);
        }

        public UnityObject[] Items
        {
            get
            {
                RemoveInvalid();
                var arr = new UnityObject[counter.Count];
                counter.CopyTo(arr, 0);
                return arr;
            }
        }

        public void Clear()
        {
            if (Count == 0) return;
            counter.Clear();
            OnFilled?.Invoke(counter.Count);
        }

        public bool Contains(UnityObject item)
        {
            return counter.Contains(item);
        }

        public void CopyTo(UnityObject[] array, int arrayIndex)
        {
            counter.CopyTo(array, arrayIndex);
        }

        public static Counter operator +(Counter c, UnityObject obj)
        {
            c.Add(obj);
            return c;
        }

        public static Counter operator -(Counter c, UnityObject obj)
        {
            c.Remove(obj);
            return c;
        }

        public static bool operator >(Counter c, int count) => c.Count > count;
        public static bool operator <(Counter c, int count) => c.Count < count;
        public static bool operator ==(Counter c, int count) => c.Count == count;
        public static bool operator !=(Counter c, int count) => !(c == count);
        public static bool operator >=(Counter c, int count) => !(c < count);
        public static bool operator <=(Counter c, int count) => !(c > count);
    }


    /// <summary>
    /// A semaphore like structure for the game to count something.
    /// </summary>
    [Serializable]
    public class Counter<T> : ICounter<T>
    {
        private List<T> counter;
        public event Action<int> OnFilled;
        T[] cachedItems;

        List<T> ICounter<T>.Counter => counter;
        public int Count => counter.Count;


        public Counter()
        {
            counter = new List<T>();
        }

        public Counter(Action<int> baseAction) : this()
        {
            OnFilled = baseAction;
        }

        public bool Add(T obj)
        {
            // not a valid object
            if (obj == null) return false;
            if (counter.Contains(obj))
            {
                return false;
            }
            counter.Add(obj);
            cachedItems = null;
            OnFilled?.Invoke(counter.Count);
            return true;
        }

        public bool Remove(T obj)
        {
            if (obj == null) return false;
            bool v = counter.Remove(obj);
            cachedItems = null;
            if (v) OnFilled?.Invoke(counter.Count);
            return v;
        }

        public void Clear()
        {
            if (Count == 0) return;
            counter.Clear();
            cachedItems = Array.Empty<T>();
            OnFilled?.Invoke(counter.Count);
        }

        public bool Contains(T item)
        {
            return counter.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            counter.CopyTo(array, arrayIndex);
        }

        public T[] Items
        {
            get
            {
                if (cachedItems != null) return cachedItems;
                var arr = cachedItems = new T[counter.Count];
                counter.CopyTo(arr, 0);
                return arr;
            }
        }

        public static Counter<T> operator +(Counter<T> c, T obj)
        {
            c.Add(obj);
            return c;
        }

        public static Counter<T> operator -(Counter<T> c, T obj)
        {
            c.Remove(obj);
            return c;
        }

        public static bool operator >(Counter<T> c, int count) => c.Count > count;
        public static bool operator <(Counter<T> c, int count) => c.Count < count;
        public static bool operator ==(Counter<T> c, int count) => c.Count == count;
        public static bool operator !=(Counter<T> c, int count) => !(c == count);
        public static bool operator >=(Counter<T> c, int count) => !(c < count);
        public static bool operator <=(Counter<T> c, int count) => !(c > count);
    }
}