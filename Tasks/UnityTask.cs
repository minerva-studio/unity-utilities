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
#if UNITY_2023_1_OR_NEWER
        [Obsolete("Use Awaitable instead")]
#endif
        public static async Task WaitForSeconds(float seconds, CancellationToken cancellationToken = default)
        {
            float current = 0;
            while (current < seconds && !cancellationToken.IsCancellationRequested)
            {
                current += Time.deltaTime;
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Similar to <see cref="UnityEngine.WaitForSecondsRealtime"/>
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static async Task WaitForSecondsRealtime(float seconds, CancellationToken cancellationToken = default)
        {
            float current = 0;
            while (current < seconds && !cancellationToken.IsCancellationRequested)
            {
                current += Time.unscaledDeltaTime;
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        public static async Task WaitWhile(Func<bool> value, CancellationToken destroyCancellationToken = default)
        {
            while (value() && !destroyCancellationToken.IsCancellationRequested)
            {
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }
        }




        public static Task AsTask(this UnityEngine.Object obj)
        {
            if (!obj) return Task.CompletedTask;
            if (obj is MonoBehaviour monoBehaviour)
            {
                return AsTask(monoBehaviour);
            }
            return AwaitObjectLifetime(obj);

            static async Task AwaitObjectLifetime(UnityEngine.Object obj)
            {
                while (obj)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
        }

        public static Task AsTask(this MonoBehaviour monoBehaviour)
        {
            if (!monoBehaviour)
            {
                return Task.CompletedTask;
            }
            var token = monoBehaviour.destroyCancellationToken;
            if (token.IsCancellationRequested) return Task.CompletedTask;

            var tcs = new TaskCompletionSource<object>();
            token.Register(() => tcs.TrySetResult(null));
            return tcs.Task;
        }
    }
}