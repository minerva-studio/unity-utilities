using System;
using System.Threading;
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
        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSeconds"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSeconds(float seconds, CancellationToken cancellationToken)
        {
            float current = 0;
            while (current < seconds && !cancellationToken.IsCancellationRequested)
            {
                current += Time.deltaTime;
                await Task.Yield();
            }
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSecondsRealtime"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSecondsRealtime(float seconds)
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return;
#endif
            float current = 0;
            while (current < seconds)
            {
                current += Time.unscaledDeltaTime;
                await Task.Yield();
            }
        }

        public static Task WaitWhile(Func<bool> value)
        {
            return WaitWhile(value, CancellationToken.None);
        }

        public static async Task WaitWhile(Func<bool> value, CancellationToken destroyCancellationToken)
        {
            while (value() && !destroyCancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }



        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSecondsRealtime"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSecondsRealtime(float seconds, CancellationToken cancellationToken)
        {
            float current = 0;
            while (current < seconds && !cancellationToken.IsCancellationRequested)
            {
                current += Time.unscaledDeltaTime;
                await Task.Yield();
            }
        }

        public static async Task AsTask(this UnityEngine.Object obj)
        {
            while (obj)
            {
                await Task.Yield();
            }
        }

        public static Task AsTask(this MonoBehaviour obj)
        {
            if (!obj)
            {
                return Task.CompletedTask;
            }
            var task = new TaskCompletionSource<object>();
            obj.destroyCancellationToken.Register(() => task.SetResult(null));
            return task.Task;
        }
    }
}