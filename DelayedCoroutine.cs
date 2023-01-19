using System;
using System.Collections;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// A fast delay coroutine (wait few second then execute)
    /// </summary>
    public struct DelayedCoroutine : IEnumerator
    {
        float delay;
        float timer;
        Action action;

        public DelayedCoroutine(float delay, Action action)
        {
            this.delay = delay;
            this.action = action;
            this.timer = 0;
        }


        public object Current => null;

        public bool MoveNext()
        {
            timer += Time.smoothDeltaTime;
            bool endOfDelay = timer < delay;
            if (endOfDelay)
            {
                action?.Invoke();
            }
            return endOfDelay;
        }

        public void Reset()
        {
            timer = 0;
        }
    }
}