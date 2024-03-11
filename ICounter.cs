using System;
using System.Collections;
using System.Collections.Generic;

namespace Minerva.Module
{
    public interface ICounter<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection
    {
        protected List<T> Counter { get; }
        public T[] Items
        {
            get
            {
                var arr = new T[((ICollection<T>)this).Count];
                Counter.CopyTo(arr, 0);
                return arr;
            }
        }

        void ICollection.CopyTo(System.Array array, int index)
        {
            Array.Copy(array, Items, ((ICollection<T>)this).Count);
        }

        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)Counter).IsReadOnly;
        void ICollection<T>.Add(T item) => Add(item);

        new bool Add(T obj);






        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Counter.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Counter.GetEnumerator();
        }


        public static ICounter<T> operator +(ICounter<T> c, T obj)
        {
            c.Add(obj);
            return c;
        }

        public static ICounter<T> operator -(ICounter<T> c, T obj)
        {
            c.Remove(obj);
            return c;
        }
    }
}