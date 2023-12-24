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
    public class PriorityEvent<T, TPriority> where T : Delegate
    {
        readonly List<(T, TPriority)> events;
        readonly IComparer<TPriority>? _comparer;
        T? combinedEvent = default;


        /// <summary>
        ///     Gets the priority comparer used by the <see cref="PriorityEvent{TElement, TPriority}" />.
        /// </summary>
        public IComparer<TPriority> Comparer => _comparer ?? Comparer<TPriority>.Default;

        public PriorityEvent()
        {
            events = new List<(T, TPriority)>();
            _comparer = default;
        }

        public PriorityEvent(IComparer<TPriority> comparer)
        {
            events = new List<(T, TPriority)>();
            _comparer = comparer;
        }





        /// <summary>
        /// the event inside this priorty event
        /// </summary>
        public T? Event { get => combinedEvent ??= CombineAllEvents(); }

        /// <summary>
        /// add an event to the priority event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="priority"></param>
        public void Add(T e, TPriority priority)
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
            events.Sort((a, b) => Comparer.Compare(a.Item2, b.Item2));
        }

        /// <summary>
        /// combine all delegate by their priority and form a single multicast delegate
        /// </summary>
        /// <returns></returns>
        private T? CombineAllEvents()
        {
            return Delegate.Combine(events.Select(e => e.Item1).ToArray()) as T;
        }


        public static PriorityEvent<T, TPriority> operator +(PriorityEvent<T, TPriority> priorityEvent, T @event)
        {
            TPriority? priority = default;
            if (priority is not null)
            {
                priorityEvent.Add(@event, priority);
            }
            else
            {
                throw new ArgumentException();
            }
            return priorityEvent;
        }

        public static PriorityEvent<T, TPriority> operator +(PriorityEvent<T, TPriority> priorityEvent, (T, TPriority) @event)
        {
            priorityEvent.Add(@event.Item1, @event.Item2);
            return priorityEvent;
        }

        public static PriorityEvent<T, TPriority> operator -(PriorityEvent<T, TPriority> priorityEvent, T @event)
        {
            priorityEvent.Remove(@event);
            return priorityEvent;
        }
    }
}
