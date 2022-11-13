#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minerva.Module
{
    /// <summary>
    /// An implementation of delegate with priority
    /// </summary>
    /// <typeparam name="T">delegate type of the event</typeparam>
    public class PriorityEvent<T> where T : Delegate
    {
        List<(T, int)> events = new List<(T, int)>();
        T? combinedEvent = default;

        /// <summary>
        /// the event inside this priorty event
        /// </summary>
        public T? Event { get => combinedEvent ??= CombineAllEvents(); }

        /// <summary>
        /// add an event to the priority event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="priority"></param>
        public void Add(T e, int priority)
        {
            events.Add((e, priority));
            Sort();
            combinedEvent = CombineAllEvents();
        }

        /// <summary>
        /// remove an event from the priority event
        /// </summary>
        /// <param name="e"></param>
        public void Remove(T e)
        {
            events.RemoveAll(p => p.Item1 == e);
            combinedEvent = CombineAllEvents();
        }

        /// <summary>
        /// sort the event
        /// </summary>
        private void Sort()
        {
            events.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        }

        /// <summary>
        /// combine all delegate by their priority and form a single multicast delegate
        /// </summary>
        /// <returns></returns>
        private T? CombineAllEvents()
        {
            return Delegate.Combine(events.Select(e => e.Item1).ToArray()) as T;
        }

        /// <summary>
        /// invoke the event, use Event?.Invoke() instead
        /// </summary>
        /// <param name="args">the argument of the delegate</param>
        /// <returns></returns>
        [Obsolete]
        public object? Invoke(params object[] args)
        {
            return Event?.DynamicInvoke(args);
        }

        public static PriorityEvent<T> operator +(PriorityEvent<T> priorityEvent, T @event)
        {
            priorityEvent.Add(@event, 1);
            return priorityEvent;
        }

        public static PriorityEvent<T> operator +(PriorityEvent<T> priorityEvent, (T, int) @event)
        {
            priorityEvent.Add(@event.Item1, @event.Item2);
            return priorityEvent;
        }

        public static PriorityEvent<T> operator -(PriorityEvent<T> priorityEvent, T @event)
        {
            priorityEvent.Remove(@event);
            return priorityEvent;
        }
    }
}
