using System;
using System.Collections;
using System.Collections.Generic;

namespace Minerva.Module
{
    public class SerializableStack<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
    {
        Stack<T> stack;

        public int Count => stack.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}