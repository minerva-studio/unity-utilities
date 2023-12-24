using System;
using System.Collections.Generic;

namespace Minerva.Module
{
    /// <summary>
    /// A list of events that always invoke and pop the first event with highest priority
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPriority"></typeparam>
    public class PriorityInvoker<T, TPriority> where T : Delegate
    {
        private readonly List<(T, TPriority)> queue;
        private readonly Comparer<TPriority> comparer;

        public int Count => queue.Count;

        public PriorityInvoker()
        {
            queue = new List<(T, TPriority)>();
            this.comparer = Comparer<TPriority>.Default;
        }

        public PriorityInvoker(Comparer<TPriority> comparer)
        {
            queue = new List<(T, TPriority)>();
            this.comparer = comparer;
        }

        public void Enqueue(T action, TPriority priority)
        {
            queue.Add((action, priority));
            queue.Sort((a, b) => comparer.Compare(a.Item2, b.Item2));
        }

        public void Invoke(params object[] args)
        {
            var (action, priority) = queue[^1];
            queue.RemoveAt(queue.Count - 1);
            action.DynamicInvoke(args);
        }

        public void Remove(T action)
        {
            queue.RemoveAll(q => q.Item1 == action);
        }


        public static PriorityInvoker<T, TPriority> operator +(PriorityInvoker<T, TPriority> priorityInvoker, T action)
        {
            TPriority priority = default;
            priorityInvoker.Enqueue(action, priority);
            return priorityInvoker;
        }

        public static PriorityInvoker<T, TPriority> operator +(PriorityInvoker<T, TPriority> priorityInvoker, (T, TPriority) action)
        {
            priorityInvoker.Enqueue(action.Item1, action.Item2);
            return priorityInvoker;
        }

        public static PriorityInvoker<T, TPriority> operator -(PriorityInvoker<T, TPriority> priorityInvoker, T action)
        {
            priorityInvoker.Remove(action);
            return priorityInvoker;
        }
    }
}
