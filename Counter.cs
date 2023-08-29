using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Minerva.Module
{
    /// <summary>
    /// A semaphore like structure for the game to control game pause etc.
    /// </summary>
    [Serializable]
    public class Counter
    {
        private HashSet<UnityObject> counter;
        public event Action<int> OnFilled;
        public int Count => counter.Count;

        public Counter()
        {
            counter = new HashSet<UnityObject>();
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
            bool v = counter.Add(obj);
            if (v) OnFilled?.Invoke(counter.Count);
            return v;
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
            counter.RemoveWhere(o => !o);
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
    }


    /// <summary>
    /// A semaphore like structure for the game to control game pause etc.
    /// </summary>
    [Serializable]
    public class Counter<T>
    {
        private HashSet<T> counter;
        public event Action<int> OnFilled;
        public int Count => counter.Count;

        public Counter()
        {
            counter = new HashSet<T>();
        }

        public Counter(Action<int> baseAction) : this()
        {
            OnFilled = baseAction;
        }

        public bool Add(T obj)
        {
            // not a valid object
            if (obj == null) return false;
            bool v = counter.Add(obj);
            if (v) OnFilled?.Invoke(counter.Count);
            return v;
        }

        public bool Remove(T obj)
        {
            if (obj == null) return false;
            bool v = counter.Remove(obj);
            if (v) OnFilled?.Invoke(counter.Count);
            return v;
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
    }
}