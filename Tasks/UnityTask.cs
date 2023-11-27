using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Minerva.Module.Tasks
{
    /// <summary>
    /// Tasks that behave like some of the Unity Coroutine <see cref="YieldInstruction"/> and completely written in Task
    /// </summary>
    public static class UnityTask
    {
        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitUntil"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task WaitUntil(Func<bool> predicate)
        {
            var tcs = new TaskCompletionSource<bool>();

            async void CheckPredicate()
            {
                while (!predicate())
                {
                    // Retry after a short delay
                    await Task.Yield();
                }
                tcs.SetResult(true);

            }
            CheckPredicate();
            return tcs.Task;
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitWhile"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task WaitWhile(Func<bool> predicate)
        {
            var tcs = new TaskCompletionSource<bool>();

            async void CheckPredicate()
            {
                while (predicate())
                {
                    // Retry after a short delay
                    await Task.Yield();
                }
                tcs.SetResult(true);

            }
            CheckPredicate();
            return tcs.Task;
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSecondsRealtime"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Task WaitForSecondsRealtime(float seconds)
        {
            return Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSeconds"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSeconds(float seconds)
        {
            float current = 0;
            while (current < seconds)
            {
                current += Time.deltaTime;
                await Task.Yield();
            }
        }
    }
}